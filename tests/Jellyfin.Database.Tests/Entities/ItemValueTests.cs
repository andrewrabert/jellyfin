using System;
using System.Collections.Generic;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ItemValueTests
    {
        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var id = Guid.NewGuid();
            var itemValue = new ItemValue
            {
                ItemValueId = id,
                Type = ItemValueType.Genre,
                Value = "Sci-Fi",
                CleanValue = "sci-fi"
            };

            Assert.Equal(id, itemValue.ItemValueId);
            Assert.Equal(ItemValueType.Genre, itemValue.Type);
            Assert.Equal("Sci-Fi", itemValue.Value);
            Assert.Equal("sci-fi", itemValue.CleanValue);
        }

        [Fact]
        public void BaseItemsMap_DefaultsToNull()
        {
            var itemValue = new ItemValue
            {
                ItemValueId = Guid.NewGuid(),
                Type = ItemValueType.Artist,
                Value = "Artist",
                CleanValue = "artist"
            };

            Assert.Null(itemValue.BaseItemsMap);
        }

        [Fact]
        public void BaseItemsMap_RoundTrips()
        {
            var map = new List<ItemValueMap>();
            var itemValue = new ItemValue
            {
                ItemValueId = Guid.NewGuid(),
                Type = ItemValueType.Tags,
                Value = "tag",
                CleanValue = "tag",
                BaseItemsMap = map
            };

            Assert.Same(map, itemValue.BaseItemsMap);
        }
    }
}
