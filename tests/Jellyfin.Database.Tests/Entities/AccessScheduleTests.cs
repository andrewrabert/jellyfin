using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class AccessScheduleTests
    {
        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var userId = Guid.NewGuid();
            var schedule = new AccessSchedule(DynamicDayOfWeek.Friday, 8.5, 21.25, userId);

            Assert.Equal(DynamicDayOfWeek.Friday, schedule.DayOfWeek);
            Assert.Equal(8.5, schedule.StartHour);
            Assert.Equal(21.25, schedule.EndHour);
            Assert.Equal(userId, schedule.UserId);
        }

        [Fact]
        public void Constructor_IdDefaultsToZero()
        {
            var schedule = new AccessSchedule(DynamicDayOfWeek.Sunday, 0, 24, Guid.NewGuid());

            Assert.Equal(0, schedule.Id);
        }

        [Fact]
        public void MutableProperties_RoundTrip()
        {
            var schedule = new AccessSchedule(DynamicDayOfWeek.Sunday, 0, 24, Guid.NewGuid());

            schedule.DayOfWeek = DynamicDayOfWeek.Weekend;
            schedule.StartHour = 1.5;
            schedule.EndHour = 2.75;

            Assert.Equal(DynamicDayOfWeek.Weekend, schedule.DayOfWeek);
            Assert.Equal(1.5, schedule.StartHour);
            Assert.Equal(2.75, schedule.EndHour);
        }
    }
}
