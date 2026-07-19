using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ImageInfoImageTypeTests
    {
        [Theory]
        [InlineData(ImageInfoImageType.Primary, 0)]
        [InlineData(ImageInfoImageType.Art, 1)]
        [InlineData(ImageInfoImageType.Backdrop, 2)]
        [InlineData(ImageInfoImageType.Banner, 3)]
        [InlineData(ImageInfoImageType.Logo, 4)]
        [InlineData(ImageInfoImageType.Thumb, 5)]
        [InlineData(ImageInfoImageType.Disc, 6)]
        [InlineData(ImageInfoImageType.Box, 7)]
        [InlineData(ImageInfoImageType.Screenshot, 8)]
        [InlineData(ImageInfoImageType.Menu, 9)]
        [InlineData(ImageInfoImageType.Chapter, 10)]
        [InlineData(ImageInfoImageType.BoxRear, 11)]
        [InlineData(ImageInfoImageType.Profile, 12)]
        public void Values_AreStable(ImageInfoImageType type, int expected)
        {
            Assert.Equal(expected, (int)type);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(13, Enum.GetValues<ImageInfoImageType>().Length);
        }
    }
}
