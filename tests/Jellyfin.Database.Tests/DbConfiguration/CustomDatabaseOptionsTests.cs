using System.Collections.ObjectModel;
using Jellyfin.Database.Implementations.DbConfiguration;
using Xunit;

namespace Jellyfin.Database.Tests.DbConfiguration;

public class CustomDatabaseOptionsTests
{
    private static CustomDatabaseOptions CreateOptions()
        => new CustomDatabaseOptions
        {
            PluginName = "MyPlugin",
            PluginAssembly = "MyPlugin.Assembly",
            ConnectionString = "Data Source=custom.db"
        };

    [Fact]
    public void Properties_RoundTrip()
    {
        var options = CreateOptions();

        Assert.Equal("MyPlugin", options.PluginName);
        Assert.Equal("MyPlugin.Assembly", options.PluginAssembly);
        Assert.Equal("Data Source=custom.db", options.ConnectionString);
    }

    [Fact]
    public void Options_DefaultsToEmptyCollection()
    {
        var options = CreateOptions();

        Assert.NotNull(options.Options);
        Assert.Empty(options.Options);
    }

    [Fact]
    public void Options_CanBeReplacedAndPopulated()
    {
        var options = CreateOptions();

        options.Options = new Collection<CustomDatabaseOption>
        {
            new CustomDatabaseOption { Key = "k", Value = "v" }
        };

        var option = Assert.Single(options.Options);
        Assert.Equal("k", option.Key);
        Assert.Equal("v", option.Value);
    }
}
