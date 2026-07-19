using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests;

public sealed class QueryPartitionHelpersTests : IDisposable
{
    private const int ItemCount = 5;

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<JellyfinDbContext> _dbOptions;

    public QueryPartitionHelpersTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var ctx = CreateDbContext();
        ctx.Database.EnsureCreated();

        for (var i = 0; i < ItemCount; i++)
        {
            ctx.BaseItems.Add(new BaseItemEntity
            {
                Id = Guid.Parse($"70000000-0000-0000-0000-00000000000{i + 1}"),
                Type = "TestItem"
            });
        }

        ctx.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public async Task PartitionAsync_ReturnsAllItemsInOrder()
    {
        using var ctx = CreateDbContext();

        var items = new List<Guid>();
        await foreach (var item in ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id).PartitionAsync(2, cancellationToken: TestContext.Current.CancellationToken))
        {
            items.Add(item.Id);
        }

        Assert.Equal(ItemCount, items.Count);
        Assert.Equal(items.OrderBy(id => id).ToList(), items);
    }

    [Fact]
    public async Task PartitionEagerAsync_ReturnsAllItemsInOrder()
    {
        using var ctx = CreateDbContext();

        var items = new List<Guid>();
        await foreach (var item in ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id).PartitionEagerAsync(2, cancellationToken: TestContext.Current.CancellationToken))
        {
            items.Add(item.Id);
        }

        Assert.Equal(ItemCount, items.Count);
        Assert.Equal(items.OrderBy(id => id).ToList(), items);
    }

    [Fact]
    public async Task PartitionAsync_PartitionSizeLargerThanSource_ReturnsAllItems()
    {
        using var ctx = CreateDbContext();

        var count = 0;
        await foreach (var item in ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id).PartitionAsync(100, cancellationToken: TestContext.Current.CancellationToken))
        {
            count++;
        }

        Assert.Equal(ItemCount, count);
    }

    [Fact]
    public async Task WithPartitionProgress_InvokesPartitionCallbacks()
    {
        using var ctx = CreateDbContext();

        var begun = new List<int>();
        var ended = new List<int>();

        var progressable = ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id)
            .WithPartitionProgress(i => begun.Add(i), (i, elapsed) => ended.Add(i));

        var count = 0;
        await foreach (var item in progressable.PartitionAsync(2, cancellationToken: TestContext.Current.CancellationToken))
        {
            count++;
        }

        Assert.Equal(ItemCount, count);
        Assert.Equal(new[] { 0, 1, 2 }, begun);
        Assert.Equal(new[] { 0, 1, 2 }, ended);
    }

    [Fact]
    public async Task WithItemProgress_InvokesItemCallbacks()
    {
        using var ctx = CreateDbContext();

        var begun = new List<(int Iteration, int Index)>();
        var ended = new List<(int Iteration, int Index)>();

        var progressable = ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id)
            .WithItemProgress(
                (item, iteration, index) => begun.Add((iteration, index)),
                (item, iteration, index, elapsed) => ended.Add((iteration, index)));

        var count = 0;
        await foreach (var item in progressable.PartitionAsync(2, cancellationToken: TestContext.Current.CancellationToken))
        {
            count++;
        }

        Assert.Equal(ItemCount, count);
        Assert.Equal(new[] { (0, 0), (0, 1), (1, 0), (1, 1), (2, 0) }, begun);
        Assert.Equal(begun, ended);
    }

    [Fact]
    public async Task WithIndex_AddsSequentialIndexes()
    {
        var indexed = new List<(string Item, int Index)>();
        await foreach (var entry in GetSourceAsync().WithIndex())
        {
            indexed.Add(entry);
        }

        Assert.Equal(new[] { ("a", 0), ("b", 1), ("c", 2) }, indexed);
    }

    private static async IAsyncEnumerable<string> GetSourceAsync()
    {
        yield return "a";
        yield return "b";
        yield return "c";
        await Task.CompletedTask;
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
