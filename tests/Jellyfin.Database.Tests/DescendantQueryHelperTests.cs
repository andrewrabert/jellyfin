using System;
using System.Linq;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Implementations.MatchCriteria;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests;

public sealed class DescendantQueryHelperTests : IDisposable
{
    private static readonly Guid _rootId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid _childFolderId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid _grandChildId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid _linkedItemId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid _unrelatedId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<JellyfinDbContext> _dbOptions;

    public DescendantQueryHelperTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = CreateDbContext();
        ctx.Database.EnsureCreated();
        SeedHierarchy(ctx);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void GetAllDescendantIds_NullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetAllDescendantIds(null!, _rootId));
    }

    [Fact]
    public void GetOwnedDescendantIds_NullContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetOwnedDescendantIds(null!, _rootId));
    }

    [Fact]
    public void GetOwnedDescendantIdsBatch_NullArguments_Throw()
    {
        using var ctx = CreateDbContext();

        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetOwnedDescendantIdsBatch(null!, [_rootId]));
        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetOwnedDescendantIdsBatch(ctx, null!));
    }

    [Fact]
    public void GetFolderIdsMatching_NullArguments_Throw()
    {
        using var ctx = CreateDbContext();

        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetFolderIdsMatching(null!, new HasSubtitles()));
        Assert.Throws<ArgumentNullException>(() => DescendantQueryHelper.GetFolderIdsMatching(ctx, null!));
    }

    [Fact]
    public void GetFolderIdsMatching_UnknownCriteria_Throws()
    {
        using var ctx = CreateDbContext();

        Assert.Throws<ArgumentOutOfRangeException>(() => DescendantQueryHelper.GetFolderIdsMatching(ctx, new UnknownCriteria()));
    }

    [Fact]
    public void GetAllDescendantIds_IncludesOwnedAndLinkedDescendants()
    {
        using var ctx = CreateDbContext();

        var descendants = DescendantQueryHelper.GetAllDescendantIds(ctx, _rootId).ToHashSet();

        Assert.Equal(new[] { _childFolderId, _grandChildId, _linkedItemId }.ToHashSet(), descendants);
    }

    [Fact]
    public void GetAllDescendantIds_ExcludesParentItself()
    {
        using var ctx = CreateDbContext();

        Assert.DoesNotContain(_rootId, DescendantQueryHelper.GetAllDescendantIds(ctx, _rootId));
    }

    [Fact]
    public void GetOwnedDescendantIds_ExcludesLinkedChildren()
    {
        using var ctx = CreateDbContext();

        var descendants = DescendantQueryHelper.GetOwnedDescendantIds(ctx, _rootId).ToHashSet();

        Assert.Equal(new[] { _childFolderId, _grandChildId }.ToHashSet(), descendants);
    }

    [Fact]
    public void GetOwnedDescendantIdsBatch_EmptyInput_ReturnsEmpty()
    {
        using var ctx = CreateDbContext();

        Assert.Empty(DescendantQueryHelper.GetOwnedDescendantIdsBatch(ctx, Array.Empty<Guid>()));
    }

    [Fact]
    public void GetOwnedDescendantIdsBatch_ExcludesSeeds()
    {
        using var ctx = CreateDbContext();

        var descendants = DescendantQueryHelper.GetOwnedDescendantIdsBatch(ctx, [_rootId, _childFolderId]);

        Assert.Equal(new[] { _grandChildId }.ToHashSet(), descendants);
    }

    [Fact]
    public void GetFolderIdsMatching_HasSubtitles_ReturnsAncestorsOfSubtitledItems()
    {
        using var ctx = CreateDbContext();

        var folders = DescendantQueryHelper.GetFolderIdsMatching(ctx, new HasSubtitles()).ToHashSet();

        Assert.Equal(new[] { _rootId, _childFolderId }.ToHashSet(), folders);
    }

    [Fact]
    public void GetFolderIdsMatching_HasChapterImages_ReturnsAncestorsOfItemsWithImages()
    {
        using var ctx = CreateDbContext();

        var folders = DescendantQueryHelper.GetFolderIdsMatching(ctx, new HasChapterImages()).ToHashSet();

        Assert.Equal(new[] { _rootId, _childFolderId }.ToHashSet(), folders);
    }

    [Fact]
    public void GetFolderIdsMatching_HasMediaStreamType_MatchesLanguageThroughLinkedChildren()
    {
        using var ctx = CreateDbContext();

        var folders = DescendantQueryHelper.GetFolderIdsMatching(ctx, new HasMediaStreamType(MediaStreamTypeEntity.Audio, "jpn")).ToHashSet();

        Assert.Equal(new[] { _rootId }.ToHashSet(), folders);
    }

    [Fact]
    public void GetFolderIdsMatching_HasMediaStreamType_UndMatchesMissingLanguage()
    {
        using var ctx = CreateDbContext();

        var folders = DescendantQueryHelper.GetFolderIdsMatching(ctx, new HasMediaStreamType(MediaStreamTypeEntity.Audio, "und")).ToHashSet();

        Assert.Equal(new[] { _rootId }.ToHashSet(), folders);
    }

    [Fact]
    public void GetFolderIdsMatching_HasMediaStreamType_FiltersOnIsExternal()
    {
        using var ctx = CreateDbContext();

        var externalMatches = DescendantQueryHelper.GetFolderIdsMatching(
            ctx,
            new HasMediaStreamType(MediaStreamTypeEntity.Subtitle, "eng", true));

        Assert.Empty(externalMatches);

        var internalMatches = DescendantQueryHelper.GetFolderIdsMatching(
            ctx,
            new HasMediaStreamType(MediaStreamTypeEntity.Subtitle, "eng", false)).ToHashSet();

        Assert.Equal(new[] { _rootId, _childFolderId }.ToHashSet(), internalMatches);
    }

    private static void SeedHierarchy(JellyfinDbContext ctx)
    {
        ctx.BaseItems.AddRange(
            CreateItem(_rootId, isFolder: true),
            CreateItem(_childFolderId, isFolder: true),
            CreateItem(_grandChildId, isFolder: false),
            CreateItem(_linkedItemId, isFolder: false),
            CreateItem(_unrelatedId, isFolder: false));

        ctx.AncestorIds.AddRange(
            new AncestorId { ParentItemId = _rootId, ItemId = _childFolderId, ParentItem = null!, Item = null! },
            new AncestorId { ParentItemId = _childFolderId, ItemId = _grandChildId, ParentItem = null!, Item = null! });

        ctx.LinkedChildren.Add(new LinkedChildEntity
        {
            ParentId = _rootId,
            ChildId = _linkedItemId,
            ChildType = LinkedChildType.Manual
        });

        ctx.MediaStreamInfos.AddRange(
            new MediaStreamInfo
            {
                ItemId = _grandChildId,
                Item = null!,
                StreamIndex = 0,
                StreamType = MediaStreamTypeEntity.Subtitle,
                Language = "eng",
                IsExternal = false
            },
            new MediaStreamInfo
            {
                ItemId = _linkedItemId,
                Item = null!,
                StreamIndex = 0,
                StreamType = MediaStreamTypeEntity.Audio,
                Language = "jpn",
                IsExternal = false
            },
            new MediaStreamInfo
            {
                ItemId = _linkedItemId,
                Item = null!,
                StreamIndex = 1,
                StreamType = MediaStreamTypeEntity.Audio,
                Language = null,
                IsExternal = false
            });

        ctx.Chapters.AddRange(
            new Chapter
            {
                ItemId = _grandChildId,
                Item = null!,
                ChapterIndex = 0,
                StartPositionTicks = 0,
                ImagePath = "/chapters/1.jpg"
            },
            new Chapter
            {
                ItemId = _unrelatedId,
                Item = null!,
                ChapterIndex = 0,
                StartPositionTicks = 0,
                ImagePath = null
            });

        ctx.SaveChanges();
    }

    private static BaseItemEntity CreateItem(Guid id, bool isFolder)
        => new BaseItemEntity
        {
            Id = id,
            Type = "TestItem",
            IsFolder = isFolder
        };

    private JellyfinDbContext CreateDbContext()
    {
        return new JellyfinDbContext(
            _dbOptions,
            NullLogger<JellyfinDbContext>.Instance,
            new SqliteDatabaseProvider(null!, NullLogger<SqliteDatabaseProvider>.Instance),
            new NoLockBehavior(NullLogger<NoLockBehavior>.Instance));
    }

    private sealed record UnknownCriteria : FolderMatchCriteria;
}
