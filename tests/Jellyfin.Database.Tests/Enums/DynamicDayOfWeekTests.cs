using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class DynamicDayOfWeekTests
{
    [Theory]
    [InlineData(DynamicDayOfWeek.Sunday, 0)]
    [InlineData(DynamicDayOfWeek.Monday, 1)]
    [InlineData(DynamicDayOfWeek.Tuesday, 2)]
    [InlineData(DynamicDayOfWeek.Wednesday, 3)]
    [InlineData(DynamicDayOfWeek.Thursday, 4)]
    [InlineData(DynamicDayOfWeek.Friday, 5)]
    [InlineData(DynamicDayOfWeek.Saturday, 6)]
    [InlineData(DynamicDayOfWeek.Everyday, 7)]
    [InlineData(DynamicDayOfWeek.Weekday, 8)]
    [InlineData(DynamicDayOfWeek.Weekend, 9)]
    public void Member_HasExpectedValue(DynamicDayOfWeek member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(10, Enum.GetValues<DynamicDayOfWeek>().Length);
    }
}
