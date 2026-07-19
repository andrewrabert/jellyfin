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

public sealed class ProgressablePartitionReportingTests : IDisposable
{
    private const int ItemCount = 3;

    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<JellyfinDbContext> _dbOptions;

    public ProgressablePartitionReportingTests()
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
    public async Task PartitionAsync_ReportsPartitionTimings()
    {
        using var ctx = CreateDbContext();

        var partitionDurations = new List<TimeSpan>();
        var progressable = ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id)
            .WithPartitionProgress(endPartition: (iteration, elapsed) => partitionDurations.Add(elapsed));

        await foreach (var item in progressable.PartitionAsync(2, TestContext.Current.CancellationToken))
        {
        }

        Assert.Equal(2, partitionDurations.Count);
        Assert.All(partitionDurations, elapsed => Assert.True(elapsed >= TimeSpan.Zero));
    }

    [Fact]
    public async Task PartitionAsync_ReportsItemEntitiesAndTimings()
    {
        using var ctx = CreateDbContext();

        var begunItems = new List<Guid>();
        var endedItems = new List<(Guid Id, TimeSpan Elapsed)>();

        var progressable = ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id)
            .WithItemProgress(
                (item, iteration, index) => begunItems.Add(item.Id),
                (item, iteration, index, elapsed) => endedItems.Add((item.Id, elapsed)));

        var yielded = new List<Guid>();
        await foreach (var item in progressable.PartitionAsync(2, TestContext.Current.CancellationToken))
        {
            yielded.Add(item.Id);
        }

        Assert.Equal(yielded, begunItems);
        Assert.Equal(yielded, endedItems.Select(e => e.Id).ToList());
        Assert.All(endedItems, e => Assert.True(e.Elapsed >= TimeSpan.Zero));
    }

    [Fact]
    public async Task ChainedWith_CombinesPartitionAndItemCallbacks()
    {
        using var ctx = CreateDbContext();

        var partitionsBegun = 0;
        var itemsBegun = 0;

        var progressable = ctx.BaseItems.Where(e => e.Type == "TestItem").OrderBy(e => e.Id)
            .WithPartitionProgress(_ => partitionsBegun++)
            .WithItemProgress((item, iteration, index) => itemsBegun++);

        await foreach (var item in progressable.PartitionEagerAsync(2, TestContext.Current.CancellationToken))
        {
        }

        Assert.Equal(2, partitionsBegun);
        Assert.Equal(ItemCount, itemsBegun);
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
