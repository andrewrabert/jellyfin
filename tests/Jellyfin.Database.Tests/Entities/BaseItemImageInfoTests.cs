using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemImageInfoTests
    {
        private static BaseItemImageInfo CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            return new BaseItemImageInfo
            {
                Id = Guid.NewGuid(),
                Path = "/images/primary.jpg",
                ItemId = item.Id,
                Item = item
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var imageInfo = CreateInstance();

            Assert.Equal("/images/primary.jpg", imageInfo.Path);
            Assert.Equal(imageInfo.Item.Id, imageInfo.ItemId);
            Assert.NotEqual(Guid.Empty, imageInfo.Id);
        }

        [Fact]
        public void OptionalProperties_HaveExpectedDefaults()
        {
            var imageInfo = CreateInstance();

            Assert.Null(imageInfo.DateModified);
            Assert.Null(imageInfo.Blurhash);
            Assert.Equal(ImageInfoImageType.Primary, imageInfo.ImageType);
            Assert.Equal(0, imageInfo.Width);
            Assert.Equal(0, imageInfo.Height);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var modified = DateTime.UtcNow;
            var blurhash = new byte[] { 1, 2, 3 };
            var imageInfo = CreateInstance();

            imageInfo.DateModified = modified;
            imageInfo.ImageType = ImageInfoImageType.Backdrop;
            imageInfo.Width = 1920;
            imageInfo.Height = 1080;
            imageInfo.Blurhash = blurhash;

            Assert.Equal(modified, imageInfo.DateModified);
            Assert.Equal(ImageInfoImageType.Backdrop, imageInfo.ImageType);
            Assert.Equal(1920, imageInfo.Width);
            Assert.Equal(1080, imageInfo.Height);
            Assert.Same(blurhash, imageInfo.Blurhash);
        }
    }
}
