using Jellyfin.Database.Implementations.DbConfiguration;
using Xunit;

namespace Jellyfin.Database.Tests.DbConfiguration;

public class DatabaseConfigurationOptionsTests
{
    [Fact]
    public void DatabaseType_RoundTrips()
    {
        var options = new DatabaseConfigurationOptions { DatabaseType = "Jellyfin-SQLite" };

        Assert.Equal("Jellyfin-SQLite", options.DatabaseType);
    }

    [Fact]
    public void CustomProviderOptions_DefaultsToNull()
    {
        var options = new DatabaseConfigurationOptions { DatabaseType = "Jellyfin-SQLite" };

        Assert.Null(options.CustomProviderOptions);
    }

    [Fact]
    public void LockingBehavior_DefaultsToNoLock()
    {
        var options = new DatabaseConfigurationOptions { DatabaseType = "Jellyfin-SQLite" };

        Assert.Equal(DatabaseLockingBehaviorTypes.NoLock, options.LockingBehavior);
    }

    [Fact]
    public void LockingBehavior_RoundTrips()
    {
        var options = new DatabaseConfigurationOptions
        {
            DatabaseType = "Jellyfin-SQLite",
            LockingBehavior = DatabaseLockingBehaviorTypes.Optimistic
        };

        Assert.Equal(DatabaseLockingBehaviorTypes.Optimistic, options.LockingBehavior);
    }

    [Fact]
    public void CustomProviderOptions_RoundTrips()
    {
        var custom = new CustomDatabaseOptions
        {
            PluginName = "Plugin",
            PluginAssembly = "Plugin.Assembly",
            ConnectionString = "Data Source=custom.db"
        };

        var options = new DatabaseConfigurationOptions
        {
            DatabaseType = "custom",
            CustomProviderOptions = custom
        };

        Assert.Same(custom, options.CustomProviderOptions);
    }
}
