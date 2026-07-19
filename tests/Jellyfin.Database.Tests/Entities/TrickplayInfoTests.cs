using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class TrickplayInfoTests
    {
        [Fact]
        public void Properties_HaveExpectedDefaults()
        {
            var info = new TrickplayInfo();

            Assert.Equal(Guid.Empty, info.ItemId);
            Assert.Equal(0, info.Width);
            Assert.Equal(0, info.Height);
            Assert.Equal(0, info.TileWidth);
            Assert.Equal(0, info.TileHeight);
            Assert.Equal(0, info.ThumbnailCount);
            Assert.Equal(0, info.Interval);
            Assert.Equal(0, info.Bandwidth);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var itemId = Guid.NewGuid();
            var info = new TrickplayInfo
            {
                ItemId = itemId,
                Width = 320,
                Height = 180,
                TileWidth = 10,
                TileHeight = 10,
                ThumbnailCount = 100,
                Interval = 10000,
                Bandwidth = 512000
            };

            Assert.Equal(itemId, info.ItemId);
            Assert.Equal(320, info.Width);
            Assert.Equal(180, info.Height);
            Assert.Equal(10, info.TileWidth);
            Assert.Equal(10, info.TileHeight);
            Assert.Equal(100, info.ThumbnailCount);
            Assert.Equal(10000, info.Interval);
            Assert.Equal(512000, info.Bandwidth);
        }
    }
}
