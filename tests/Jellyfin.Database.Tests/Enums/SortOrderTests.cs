using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class SortOrderTests
{
    [Theory]
    [InlineData(SortOrder.Ascending, 0)]
    [InlineData(SortOrder.Descending, 1)]
    public void Member_HasExpectedValue(SortOrder member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(2, Enum.GetValues<SortOrder>().Length);
    }
}
