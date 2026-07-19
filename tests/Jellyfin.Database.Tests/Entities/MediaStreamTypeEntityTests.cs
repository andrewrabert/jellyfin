using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class MediaStreamTypeEntityTests
    {
        [Theory]
        [InlineData(MediaStreamTypeEntity.Audio, 0)]
        [InlineData(MediaStreamTypeEntity.Video, 1)]
        [InlineData(MediaStreamTypeEntity.Subtitle, 2)]
        [InlineData(MediaStreamTypeEntity.EmbeddedImage, 3)]
        [InlineData(MediaStreamTypeEntity.Data, 4)]
        [InlineData(MediaStreamTypeEntity.Lyric, 5)]
        public void Values_AreStable(MediaStreamTypeEntity type, int expected)
        {
            Assert.Equal(expected, (int)type);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(6, Enum.GetValues<MediaStreamTypeEntity>().Length);
        }
    }
}
