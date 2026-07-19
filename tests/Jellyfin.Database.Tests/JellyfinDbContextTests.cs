using System;
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

public sealed class JellyfinDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
    private readonly CountingLockBehavior _lockBehavior;

    public JellyfinDbContextTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
            .UseSqlite(_connection)
            .Options;

        _lockBehavior = new CountingLockBehavior();

        using var ctx = CreateDbContext();
        ctx.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void DbSets_AreQueryableOnEmptyDatabase()
    {
        using var ctx = CreateDbContext();

        Assert.Empty(ctx.Users);
        Assert.Empty(ctx.AncestorIds);
        Assert.Empty(ctx.LinkedChildren);
        Assert.Empty(ctx.Chapters);
        Assert.Empty(ctx.MediaStreamInfos);
        Assert.Empty(ctx.Permissions);
        Assert.Empty(ctx.Preferences);
        Assert.Empty(ctx.ActivityLogs);
        Assert.Empty(ctx.Devices);
        Assert.Empty(ctx.TrickplayInfos);
        Assert.Empty(ctx.MediaSegments);
        Assert.Empty(ctx.UserData);
        Assert.Empty(ctx.ItemValues);
        Assert.Empty(ctx.KeyframeData);

        // The model seeds a single placeholder item for detached user data.
        var placeholder = Assert.Single(ctx.BaseItems);
        Assert.Equal("PLACEHOLDER", placeholder.Type);
    }

    [Fact]
    public void SaveChanges_PersistsEntities()
    {
        using (var ctx = CreateDbContext())
        {
            ctx.Users.Add(new User("alice", "auth", "reset"));
            ctx.SaveChanges();
        }

        using (var ctx = CreateDbContext())
        {
            var user = Assert.Single(ctx.Users);
            Assert.Equal("alice", user.Username);
        }
    }

    [Fact]
    public void SaveChanges_InvokesLockingBehavior()
    {
        using var ctx = CreateDbContext();
        ctx.Users.Add(new User("bob", "auth", "reset"));

        ctx.SaveChanges();

        Assert.Equal(1, _lockBehavior.SyncCalls);
    }

    [Fact]
    public async Task SaveChangesAsync_InvokesLockingBehavior()
    {
        using var ctx = CreateDbContext();
        ctx.Users.Add(new User("carol", "auth", "reset"));

        await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(1, _lockBehavior.AsyncCalls);
    }

    [Fact]
    public void SaveChanges_IncrementsConcurrencyTokenOnModifiedEntities()
    {
        using var ctx = CreateDbContext();
        var user = new User("dave", "auth", "reset");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        var initialRowVersion = user.RowVersion;

        user.Username = "david";
        ctx.SaveChanges();

        Assert.Equal(initialRowVersion + 1, user.RowVersion);
    }

    [Fact]
    public async Task SaveChangesAsync_IncrementsConcurrencyTokenOnModifiedEntities()
    {
        using var ctx = CreateDbContext();
        var user = new User("erin", "auth", "reset");
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        var initialRowVersion = user.RowVersion;

        user.Username = "erin2";
        await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);

        Assert.Equal(initialRowVersion + 1, user.RowVersion);
    }

    [Fact]
    public void SaveChanges_DoesNotIncrementConcurrencyTokenOnAddedEntities()
    {
        using var ctx = CreateDbContext();
        var user = new User("frank", "auth", "reset");
        ctx.Users.Add(user);

        ctx.SaveChanges();

        Assert.Equal(0u, user.RowVersion);
    }

    private JellyfinDbContext CreateDbContext()
    {
        return new JellyfinDbContext(
            _dbOptions,
            NullLogger<JellyfinDbContext>.Instance,
            new SqliteDatabaseProvider(null!, NullLogger<SqliteDatabaseProvider>.Instance),
            _lockBehavior);
    }

    private sealed class CountingLockBehavior : IEntityFrameworkCoreLockingBehavior
    {
        public int SyncCalls { get; private set; }

        public int AsyncCalls { get; private set; }

        public void Initialise(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public void OnSaveChanges(JellyfinDbContext context, Action saveChanges)
        {
            SyncCalls++;
            saveChanges();
        }

        public async Task OnSaveChangesAsync(JellyfinDbContext context, Func<Task> saveChanges)
        {
            AsyncCalls++;
            await saveChanges().ConfigureAwait(false);
        }
    }
}
