using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests;

public sealed class JellyfinQueryHelperExtensionsTests : IDisposable
{
    private static readonly Guid _movieId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid _showId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid _genreItemId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<JellyfinDbContext> _dbOptions;

    public JellyfinQueryHelperExtensionsTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = CreateDbContext();
        ctx.Database.EnsureCreated();
        Seed(ctx);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void OneOrManyExpressionBuilder_SingleValueType_BuildsEqualityCheck()
    {
        var predicate = JellyfinQueryHelperExtensions
            .OneOrManyExpressionBuilder<BaseItemEntity, Guid>([_movieId], e => e.Id)
            .Compile();

        Assert.True(predicate(new BaseItemEntity { Id = _movieId, Type = "T" }));
        Assert.False(predicate(new BaseItemEntity { Id = _showId, Type = "T" }));
    }

    [Fact]
    public void OneOrManyExpressionBuilder_MultipleValues_BuildsContainsCheck()
    {
        var predicate = JellyfinQueryHelperExtensions
            .OneOrManyExpressionBuilder<BaseItemEntity, Guid>([_movieId, _showId], e => e.Id)
            .Compile();

        Assert.True(predicate(new BaseItemEntity { Id = _movieId, Type = "T" }));
        Assert.True(predicate(new BaseItemEntity { Id = _showId, Type = "T" }));
        Assert.False(predicate(new BaseItemEntity { Id = _genreItemId, Type = "T" }));
    }

    [Fact]
    public void WhereOneOrMany_SingleValue_FiltersQuery()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems.WhereOneOrMany([_movieId], e => e.Id).ToList();

        var item = Assert.Single(result);
        Assert.Equal(_movieId, item.Id);
    }

    [Fact]
    public void WhereOneOrMany_SmallList_FiltersQuery()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems.WhereOneOrMany([_movieId, _showId], e => e.Id).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void WhereOneOrMany_LargeList_UsesParameterizedQuery()
    {
        using var ctx = CreateDbContext();

        var ids = Enumerable.Range(0, 40).Select(_ => Guid.NewGuid()).ToList();
        ids.Add(_movieId);

        var result = ctx.BaseItems.WhereOneOrMany(ids, e => e.Id).ToList();

        var item = Assert.Single(result);
        Assert.Equal(_movieId, item.Id);
    }

    [Fact]
    public void WhereHasAnyProviderIds_EmptyDictionary_ReturnsUnfilteredQuery()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems.WhereHasAnyProviderIds(new Dictionary<string, string[]>()).ToList();

        Assert.Equal(ctx.BaseItems.Count(), result.Count);
    }

    [Fact]
    public void WhereHasAnyProviderIds_MatchesProviderNameValuePair()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereHasAnyProviderIds(new Dictionary<string, string[]> { ["imdb"] = ["tt0001"] })
            .ToList();

        var item = Assert.Single(result);
        Assert.Equal(_movieId, item.Id);
    }

    [Fact]
    public void WhereHasAnyProviderId_ExistenceOnly_MatchesAnyValue()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereHasAnyProviderId(new Dictionary<string, string> { ["imdb"] = string.Empty })
            .ToList();

        var item = Assert.Single(result);
        Assert.Equal(_movieId, item.Id);
    }

    [Fact]
    public void WhereHasAnyProviderId_SpecificValue_Matches()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereHasAnyProviderId(new Dictionary<string, string> { ["tmdb"] = "200" })
            .ToList();

        var item = Assert.Single(result);
        Assert.Equal(_showId, item.Id);
    }

    [Fact]
    public void WhereHasAnyProviderId_MixedExistenceAndValue_MatchesEither()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereHasAnyProviderId(new Dictionary<string, string>
            {
                ["imdb"] = string.Empty,
                ["tmdb"] = "200"
            })
            .Select(e => e.Id)
            .ToHashSet();

        Assert.Equal(new[] { _movieId, _showId }.ToHashSet(), result);
    }

    [Fact]
    public void WhereHasAnyProviderId_EmptyDictionary_ReturnsUnfilteredQuery()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems.WhereHasAnyProviderId(new Dictionary<string, string>()).ToList();

        Assert.Equal(ctx.BaseItems.Count(), result.Count);
    }

    [Fact]
    public void WhereExcludeProviderIds_ExcludesMatchingItems()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereExcludeProviderIds(new Dictionary<string, string> { ["imdb"] = "tt0001" })
            .Select(e => e.Id)
            .ToHashSet();

        Assert.DoesNotContain(_movieId, result);
        Assert.Contains(_showId, result);
        Assert.Contains(_genreItemId, result);
    }

    [Fact]
    public void WhereExcludeProviderIds_EmptyDictionary_ReturnsUnfilteredQuery()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems.WhereExcludeProviderIds(new Dictionary<string, string>()).ToList();

        Assert.Equal(ctx.BaseItems.Count(), result.Count);
    }

    [Fact]
    public void WhereReferencedItem_MatchesItemsThroughItemValues()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereReferencedItem(ctx, ItemValueType.Genre, [_genreItemId])
            .Select(e => e.Id)
            .ToList();

        var id = Assert.Single(result);
        Assert.Equal(_movieId, id);
    }

    [Fact]
    public void WhereReferencedItem_Inverted_ExcludesMatches()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereReferencedItem(ctx, ItemValueType.Genre, [_genreItemId], invert: true)
            .Select(e => e.Id)
            .ToHashSet();

        Assert.DoesNotContain(_movieId, result);
        Assert.Contains(_showId, result);
    }

    [Fact]
    public void WhereReferencedItemMultipleTypes_MatchesAnyOfTheTypes()
    {
        using var ctx = CreateDbContext();

        var result = ctx.BaseItems
            .WhereReferencedItemMultipleTypes(ctx, [ItemValueType.Genre, ItemValueType.Tags], [_genreItemId])
            .Select(e => e.Id)
            .ToList();

        var id = Assert.Single(result);
        Assert.Equal(_movieId, id);
    }

    private static void Seed(JellyfinDbContext ctx)
    {
        ctx.BaseItems.AddRange(
            new BaseItemEntity { Id = _movieId, Type = "Movie" },
            new BaseItemEntity { Id = _showId, Type = "Show" },
            new BaseItemEntity { Id = _genreItemId, Type = "Genre", CleanName = "action" });

        ctx.BaseItemProviders.AddRange(
            new BaseItemProvider { ItemId = _movieId, Item = null!, ProviderId = "imdb", ProviderValue = "tt0001" },
            new BaseItemProvider { ItemId = _showId, Item = null!, ProviderId = "tmdb", ProviderValue = "200" });

        var genreValueId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        ctx.ItemValues.Add(new ItemValue
        {
            ItemValueId = genreValueId,
            Type = ItemValueType.Genre,
            Value = "Action",
            CleanValue = "action"
        });

        ctx.ItemValuesMap.Add(new ItemValueMap
        {
            ItemId = _movieId,
            ItemValueId = genreValueId,
            Item = null!,
            ItemValue = null!
        });

        ctx.SaveChanges();
    }

    private JellyfinDbContext CreateDbContext()
    {
        return new JellyfinDbContext(
            _dbOptions,
            NullLogger<JellyfinDbContext>.Instance,
            new SqliteDatabaseProvider(null!, NullLogger<SqliteDatabaseProvider>.Instance),
            new NoLockBehavior(NullLogger<NoLockBehavior>.Instance));
    }
}
