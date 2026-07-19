using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class MediaSegmentTests
    {
        [Fact]
        public void Properties_HaveExpectedDefaults()
        {
            var segment = new MediaSegment { SegmentProviderId = "provider" };

            Assert.Equal(Guid.Empty, segment.Id);
            Assert.Equal(Guid.Empty, segment.ItemId);
            Assert.Equal(MediaSegmentType.Unknown, segment.Type);
            Assert.Equal(0L, segment.StartTicks);
            Assert.Equal(0L, segment.EndTicks);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var id = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            var segment = new MediaSegment
            {
                Id = id,
                ItemId = itemId,
                Type = MediaSegmentType.Intro,
                StartTicks = 100L,
                EndTicks = 200L,
                SegmentProviderId = "provider"
            };

            Assert.Equal(id, segment.Id);
            Assert.Equal(itemId, segment.ItemId);
            Assert.Equal(MediaSegmentType.Intro, segment.Type);
            Assert.Equal(100L, segment.StartTicks);
            Assert.Equal(200L, segment.EndTicks);
            Assert.Equal("provider", segment.SegmentProviderId);
        }
    }
}
