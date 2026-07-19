using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class PeopleBaseItemMapTests
    {
        private static PeopleBaseItemMap CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var people = new People { Id = Guid.NewGuid(), Name = "Actor" };
            return new PeopleBaseItemMap
            {
                ItemId = item.Id,
                Item = item,
                PeopleId = people.Id,
                People = people
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var map = CreateInstance();

            Assert.Equal(map.Item.Id, map.ItemId);
            Assert.Equal(map.People.Id, map.PeopleId);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var map = CreateInstance();

            Assert.Null(map.SortOrder);
            Assert.Null(map.ListOrder);
            Assert.Null(map.Role);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var map = CreateInstance();
            map.SortOrder = 1;
            map.ListOrder = 2;
            map.Role = "Protagonist";

            Assert.Equal(1, map.SortOrder);
            Assert.Equal(2, map.ListOrder);
            Assert.Equal("Protagonist", map.Role);
        }
    }
}
