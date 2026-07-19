using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.DbConfiguration;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Database.Providers.Sqlite.ValueConverters;
using MediaBrowser.Common.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite;

public sealed class SqliteDatabaseProviderTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();
    private readonly SqliteDatabaseProvider _provider;

    public SqliteDatabaseProviderTests()
    {
        _provider = new SqliteDatabaseProvider(
            new TestApplicationPaths(_tempDirectory.FullName),
            NullLogger<SqliteDatabaseProvider>.Instance);
    }

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    private static DatabaseConfigurationOptions CreateConfiguration(params (string Key, string Value)[] options)
        => new()
        {
            DatabaseType = "Jellyfin-SQLite",
            CustomProviderOptions = options.Length == 0 ? null : new CustomDatabaseOptions
            {
                PluginName = string.Empty,
                PluginAssembly = string.Empty,
                ConnectionString = string.Empty,
                Options = new Collection<CustomDatabaseOption>(
                    options.Select(o => new CustomDatabaseOption { Key = o.Key, Value = o.Value }).ToList())
            }
        };

    [Fact]
    public void Initialise_DefaultOptions_UsesDataPathDatabaseFile()
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        _provider.Initialise(optionsBuilder, CreateConfiguration());

        var connectionString = RelationalOptionsExtension.Extract(optionsBuilder.Options).ConnectionString;
        Assert.NotNull(connectionString);
        var builder = new SqliteConnectionStringBuilder(connectionString);
        Assert.Equal(Path.Combine(_tempDirectory.FullName, "jellyfin.db"), builder.DataSource);
        Assert.True(builder.Pooling);
        Assert.Equal(60, builder.DefaultTimeout);
        Assert.Equal(SqliteCacheMode.Default, builder.Cache);
    }

    [Fact]
    public void Initialise_CustomOptions_AreAppliedToConnectionString()
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        var customPath = Path.Combine(_tempDirectory.FullName, "custom.db");

        _provider.Initialise(
            optionsBuilder,
            CreateConfiguration(
                ("path", customPath),
                ("pooling", "false"),
                ("cache", "Shared"),
                ("command-timeout", "30")));

        var connectionString = RelationalOptionsExtension.Extract(optionsBuilder.Options).ConnectionString;
        var builder = new SqliteConnectionStringBuilder(connectionString);
        Assert.Equal(customPath, builder.DataSource);
        Assert.False(builder.Pooling);
        Assert.Equal(30, builder.DefaultTimeout);
        Assert.Equal(SqliteCacheMode.Shared, builder.Cache);
    }

    [Fact]
    public void Initialise_AddsPragmaConnectionInterceptor()
    {
        var optionsBuilder = new DbContextOptionsBuilder();

        _provider.Initialise(optionsBuilder, CreateConfiguration());

        var interceptors = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()?.Interceptors;
        Assert.NotNull(interceptors);
        Assert.Single(interceptors.OfType<PragmaConnectionInterceptor>());
    }

    [Fact]
    public void OnModelCreating_AppliesUtcDateTimeKindConverter()
    {
        var modelBuilder = new ModelBuilder(SqliteConventionSetBuilder.Build());
        modelBuilder.Entity<TestEntity>();

        _provider.OnModelCreating(modelBuilder);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        Assert.NotNull(entityType);
        Assert.IsType<DateTimeKindValueConverter>(entityType.FindProperty(nameof(TestEntity.Created))!.GetValueConverter());
        Assert.IsType<DateTimeKindValueConverter>(entityType.FindProperty(nameof(TestEntity.Modified))!.GetValueConverter());
    }

    [Fact]
    public async Task MigrationBackupFast_CopiesDatabaseToBackupFolder()
    {
        var databasePath = Path.Combine(_tempDirectory.FullName, "jellyfin.db");
        await File.WriteAllTextAsync(databasePath, "database content", TestContext.Current.CancellationToken);

        var key = await _provider.MigrationBackupFast(TestContext.Current.CancellationToken);

        var backupPath = Path.Combine(_tempDirectory.FullName, "SQLiteBackups", $"{key}_jellyfin.db");
        Assert.True(File.Exists(backupPath));
        Assert.Equal("database content", await File.ReadAllTextAsync(backupPath, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RestoreBackupFast_OverwritesDatabaseFromBackup()
    {
        var databasePath = Path.Combine(_tempDirectory.FullName, "jellyfin.db");
        await File.WriteAllTextAsync(databasePath, "original", TestContext.Current.CancellationToken);
        var key = await _provider.MigrationBackupFast(TestContext.Current.CancellationToken);
        await File.WriteAllTextAsync(databasePath, "corrupted", TestContext.Current.CancellationToken);

        await _provider.RestoreBackupFast(key, TestContext.Current.CancellationToken);

        Assert.Equal("original", await File.ReadAllTextAsync(databasePath, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task RestoreBackupFast_MissingBackup_DoesNotThrow()
    {
        await _provider.RestoreBackupFast("does-not-exist", TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteBackup_RemovesBackupFile()
    {
        var databasePath = Path.Combine(_tempDirectory.FullName, "jellyfin.db");
        await File.WriteAllTextAsync(databasePath, "database content", TestContext.Current.CancellationToken);
        var key = await _provider.MigrationBackupFast(TestContext.Current.CancellationToken);
        var backupPath = Path.Combine(_tempDirectory.FullName, "SQLiteBackups", $"{key}_jellyfin.db");

        await _provider.DeleteBackup(key);

        Assert.False(File.Exists(backupPath));
    }

    [Fact]
    public async Task DeleteBackup_MissingBackup_DoesNotThrow()
    {
        await _provider.DeleteBackup("does-not-exist");
    }

    [Fact]
    public async Task PurgeDatabase_NullTableNames_Throws()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _provider.PurgeDatabase(null!, null));
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
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
