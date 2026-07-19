using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ItemValueMapTests
    {
        [Fact]
        public void Properties_RoundTrip()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var itemValue = new ItemValue
            {
                ItemValueId = Guid.NewGuid(),
                Type = ItemValueType.Genre,
                Value = "Drama",
                CleanValue = "drama"
            };

            var map = new ItemValueMap
            {
                ItemId = item.Id,
                ItemValueId = itemValue.ItemValueId,
                Item = item,
                ItemValue = itemValue
            };

            Assert.Equal(item.Id, map.ItemId);
            Assert.Equal(itemValue.ItemValueId, map.ItemValueId);
            Assert.Same(item, map.Item);
            Assert.Same(itemValue, map.ItemValue);
        }
    }
}
