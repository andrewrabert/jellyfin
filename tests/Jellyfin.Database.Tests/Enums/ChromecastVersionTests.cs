using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class ChromecastVersionTests
{
    [Theory]
    [InlineData(ChromecastVersion.Stable, 0)]
    [InlineData(ChromecastVersion.Unstable, 1)]
    public void Member_HasExpectedValue(ChromecastVersion member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(2, Enum.GetValues<ChromecastVersion>().Length);
    }
}
