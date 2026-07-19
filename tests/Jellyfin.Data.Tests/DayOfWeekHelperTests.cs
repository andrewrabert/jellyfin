using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Data.Tests
{
    public static class DayOfWeekHelperTests
    {
        [Fact]
        public static void GetDaysOfWeek_Everyday_ReturnsAllDays()
        {
            Assert.Equal(
                new[]
                {
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                },
                DayOfWeekHelper.GetDaysOfWeek(DynamicDayOfWeek.Everyday));
        }

        [Fact]
        public static void GetDaysOfWeek_Weekday_ReturnsMondayThroughFriday()
        {
            Assert.Equal(
                new[]
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday
                },
                DayOfWeekHelper.GetDaysOfWeek(DynamicDayOfWeek.Weekday));
        }

        [Fact]
        public static void GetDaysOfWeek_Weekend_ReturnsSundayAndSaturday()
        {
            Assert.Equal(
                new[] { DayOfWeek.Sunday, DayOfWeek.Saturday },
                DayOfWeekHelper.GetDaysOfWeek(DynamicDayOfWeek.Weekend));
        }

        [Theory]
        [InlineData(DynamicDayOfWeek.Sunday, DayOfWeek.Sunday)]
        [InlineData(DynamicDayOfWeek.Monday, DayOfWeek.Monday)]
        [InlineData(DynamicDayOfWeek.Tuesday, DayOfWeek.Tuesday)]
        [InlineData(DynamicDayOfWeek.Wednesday, DayOfWeek.Wednesday)]
        [InlineData(DynamicDayOfWeek.Thursday, DayOfWeek.Thursday)]
        [InlineData(DynamicDayOfWeek.Friday, DayOfWeek.Friday)]
        [InlineData(DynamicDayOfWeek.Saturday, DayOfWeek.Saturday)]
        public static void GetDaysOfWeek_SingleDay_ReturnsThatDay(DynamicDayOfWeek dynamicDay, DayOfWeek expected)
        {
            Assert.Equal(new[] { expected }, DayOfWeekHelper.GetDaysOfWeek(dynamicDay));
        }

        [Theory]
        [InlineData(DynamicDayOfWeek.Everyday, DayOfWeek.Sunday, true)]
        [InlineData(DynamicDayOfWeek.Everyday, DayOfWeek.Wednesday, true)]
        [InlineData(DynamicDayOfWeek.Everyday, DayOfWeek.Saturday, true)]
        [InlineData(DynamicDayOfWeek.Weekday, DayOfWeek.Monday, true)]
        [InlineData(DynamicDayOfWeek.Weekday, DayOfWeek.Friday, true)]
        [InlineData(DynamicDayOfWeek.Weekday, DayOfWeek.Saturday, false)]
        [InlineData(DynamicDayOfWeek.Weekday, DayOfWeek.Sunday, false)]
        [InlineData(DynamicDayOfWeek.Weekend, DayOfWeek.Saturday, true)]
        [InlineData(DynamicDayOfWeek.Weekend, DayOfWeek.Sunday, true)]
        [InlineData(DynamicDayOfWeek.Weekend, DayOfWeek.Monday, false)]
        [InlineData(DynamicDayOfWeek.Tuesday, DayOfWeek.Tuesday, true)]
        [InlineData(DynamicDayOfWeek.Tuesday, DayOfWeek.Wednesday, false)]
        public static void Contains_MatchesExpected(DynamicDayOfWeek dynamicDay, DayOfWeek day, bool expected)
        {
            Assert.Equal(expected, dynamicDay.Contains(day));
        }
    }
}
