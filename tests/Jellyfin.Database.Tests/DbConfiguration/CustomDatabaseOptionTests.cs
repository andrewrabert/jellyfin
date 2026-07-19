using Jellyfin.Database.Implementations.DbConfiguration;
using Xunit;

namespace Jellyfin.Database.Tests.DbConfiguration;

public class CustomDatabaseOptionTests
{
    [Fact]
    public void Properties_RoundTrip()
    {
        var option = new CustomDatabaseOption
        {
            Key = "timeout",
            Value = "30"
        };

        Assert.Equal("timeout", option.Key);
        Assert.Equal("30", option.Value);
    }

    [Fact]
    public void Properties_AreMutable()
    {
        var option = new CustomDatabaseOption
        {
            Key = "a",
            Value = "b"
        };

        option.Key = "c";
        option.Value = "d";

        Assert.Equal("c", option.Key);
        Assert.Equal("d", option.Value);
    }
}
