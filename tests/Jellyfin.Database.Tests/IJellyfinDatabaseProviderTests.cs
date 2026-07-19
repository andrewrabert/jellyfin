using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.DbConfiguration;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests;

public class IJellyfinDatabaseProviderTests
{
    [Fact]
    public void SqliteDatabaseProvider_ImplementsInterface()
    {
        Assert.True(typeof(IJellyfinDatabaseProvider).IsAssignableFrom(typeof(SqliteDatabaseProvider)));
    }

    [Fact]
    public void DbContextFactory_DefaultsToNullAndRoundTrips()
    {
        IJellyfinDatabaseProvider provider = new FakeDatabaseProvider();

        Assert.Null(provider.DbContextFactory);

        var factory = new FakeDbContextFactory();
        provider.DbContextFactory = factory;

        Assert.Same(factory, provider.DbContextFactory);
    }

    [Fact]
    public async Task InterfaceMembers_AreInvokableThroughImplementation()
    {
        IJellyfinDatabaseProvider provider = new FakeDatabaseProvider();

        provider.Initialise(new DbContextOptionsBuilder<JellyfinDbContext>(), new DatabaseConfigurationOptions { DatabaseType = "fake" });
        provider.OnModelCreating(new ModelBuilder());

        await provider.RunScheduledOptimisation(CancellationToken.None);
        await provider.RunShutdownTask(CancellationToken.None);

        var key = await provider.MigrationBackupFast(CancellationToken.None);
        Assert.Equal("backup-key", key);

        await provider.RestoreBackupFast(key, CancellationToken.None);
        await provider.DeleteBackup(key);

        var fake = (FakeDatabaseProvider)provider;
        Assert.Equal(new[] { "Initialise", "OnModelCreating", "RunScheduledOptimisation", "RunShutdownTask", "MigrationBackupFast", "RestoreBackupFast", "DeleteBackup" }, fake.Calls);
    }

    private sealed class FakeDatabaseProvider : IJellyfinDatabaseProvider
    {
        public List<string> Calls { get; } = new();

        public IDbContextFactory<JellyfinDbContext>? DbContextFactory { get; set; }

        public void Initialise(DbContextOptionsBuilder options, DatabaseConfigurationOptions databaseConfiguration)
            => Calls.Add(nameof(Initialise));

        public void OnModelCreating(ModelBuilder modelBuilder)
            => Calls.Add(nameof(OnModelCreating));

        public void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
            => Calls.Add(nameof(ConfigureConventions));

        public Task RunScheduledOptimisation(CancellationToken cancellationToken)
        {
            Calls.Add(nameof(RunScheduledOptimisation));
            return Task.CompletedTask;
        }

        public Task RunShutdownTask(CancellationToken cancellationToken)
        {
            Calls.Add(nameof(RunShutdownTask));
            return Task.CompletedTask;
        }

        public Task<string> MigrationBackupFast(CancellationToken cancellationToken)
        {
            Calls.Add(nameof(MigrationBackupFast));
            return Task.FromResult("backup-key");
        }

        public Task RestoreBackupFast(string key, CancellationToken cancellationToken)
        {
            Calls.Add(nameof(RestoreBackupFast));
            return Task.CompletedTask;
        }

        public Task DeleteBackup(string key)
        {
            Calls.Add(nameof(DeleteBackup));
            return Task.CompletedTask;
        }

        public Task PurgeDatabase(JellyfinDbContext dbContext, IEnumerable<string>? tableNames)
        {
            Calls.Add(nameof(PurgeDatabase));
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDbContextFactory : IDbContextFactory<JellyfinDbContext>
    {
        public JellyfinDbContext CreateDbContext() => throw new NotSupportedException();
    }
}
