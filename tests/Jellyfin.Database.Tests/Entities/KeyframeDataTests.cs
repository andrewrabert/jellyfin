using System;
using System.Collections.Generic;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class KeyframeDataTests
    {
        [Fact]
        public void OptionalProperties_HaveExpectedDefaults()
        {
            var keyframeData = new KeyframeData { ItemId = Guid.NewGuid() };

            Assert.Equal(0L, keyframeData.TotalDuration);
            Assert.Null(keyframeData.KeyframeTicks);
            Assert.Null(keyframeData.Item);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var ticks = new List<long> { 0L, 10_000_000L, 20_000_000L };

            var keyframeData = new KeyframeData
            {
                ItemId = item.Id,
                TotalDuration = 30_000_000L,
                KeyframeTicks = ticks,
                Item = item
            };

            Assert.Equal(item.Id, keyframeData.ItemId);
            Assert.Equal(30_000_000L, keyframeData.TotalDuration);
            Assert.Same(ticks, keyframeData.KeyframeTicks);
            Assert.Same(item, keyframeData.Item);
        }
    }
}
