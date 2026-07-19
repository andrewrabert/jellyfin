using System;
using System.IO;
using System.Linq;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using MediaBrowser.Common.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Migrations;

public sealed class SqliteMigrationsTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        _tempDirectory.Delete(true);
    }

    private JellyfinDbContext CreateContext()
    {
        var databasePath = Path.Combine(_tempDirectory.FullName, "jellyfin.db");
        var optionsBuilder = new DbContextOptionsBuilder<JellyfinDbContext>();
        optionsBuilder
            .UseSqlite(
                $"Data Source={databasePath};Pooling=false",
                sqliteOptions => sqliteOptions.MigrationsAssembly(typeof(SqliteDatabaseProvider).Assembly))
            .ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.NonTransactionalMigrationOperationWarning));

        return new JellyfinDbContext(
            optionsBuilder.Options,
            NullLogger<JellyfinDbContext>.Instance,
            new SqliteDatabaseProvider(new TestApplicationPaths(_tempDirectory.FullName), NullLogger<SqliteDatabaseProvider>.Instance),
            new NoLockBehavior(NullLogger<NoLockBehavior>.Instance));
    }

    [Fact]
    public void Migrate_AppliesAllMigrations_AndCreatesSchema()
    {
        using var context = CreateContext();

        context.Database.Migrate();

        Assert.NotEmpty(context.Database.GetAppliedMigrations());
        Assert.Empty(context.Database.GetPendingMigrations());

        var tables = context.Database
            .SqlQueryRaw<string>("SELECT \"name\" AS \"Value\" FROM \"sqlite_master\" WHERE \"type\" = 'table'")
            .ToList();

        Assert.Contains("__EFMigrationsHistory", tables);
        Assert.Contains("Users", tables);
        Assert.Contains("BaseItems", tables);
        Assert.Contains("ActivityLogs", tables);
        Assert.Contains("ApiKeys", tables);
        Assert.Contains("Devices", tables);
        Assert.Contains("UserData", tables);
        Assert.Contains("MediaSegments", tables);
    }

    [Fact]
    public void ModelSnapshot_HasNoPendingModelChanges()
    {
        using var context = CreateContext();

        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public void Migrate_ThenQuery_SchemaIsUsable()
    {
        using var context = CreateContext();

        context.Database.Migrate();

        Assert.Empty(context.Users);
        Assert.Empty(context.ActivityLogs);

        // The DetachUserDataInsteadOfDelete migration seeds a placeholder item.
        var placeholder = Assert.Single(context.BaseItems);
        Assert.Equal(new Guid("00000000-0000-0000-0000-000000000001"), placeholder.Id);
    }

    private sealed class TestApplicationPaths(string basePath) : IApplicationPaths
    {
        public string ProgramDataPath => basePath;

        public string WebPath => basePath;

        public string ProgramSystemPath => basePath;

        public string DataPath => basePath;

        public string ImageCachePath => basePath;

        public string PluginsPath => basePath;

        public string PluginConfigurationsPath => basePath;

        public string LogDirectoryPath => basePath;

        public string ConfigurationDirectoryPath => basePath;

        public string SystemConfigurationFilePath => Path.Combine(basePath, "system.xml");

        public string CachePath => basePath;

        public string TempDirectory => basePath;

        public string VirtualDataPath => basePath;

        public string TrickplayPath => basePath;

        public string BackupPath => basePath;

        public void MakeSanityCheckOrThrow()
        {
        }

        public void CreateAndCheckMarker(string path, string markerName, bool recursive = false)
        {
        }
    }
}
