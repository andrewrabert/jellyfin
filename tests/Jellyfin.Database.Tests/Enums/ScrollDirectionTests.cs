using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class ScrollDirectionTests
{
    [Theory]
    [InlineData(ScrollDirection.Horizontal, 0)]
    [InlineData(ScrollDirection.Vertical, 1)]
    public void Member_HasExpectedValue(ScrollDirection member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(2, Enum.GetValues<ScrollDirection>().Length);
    }
}
