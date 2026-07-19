using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ImageInfoTests
    {
        [Fact]
        public void Constructor_SetsPath()
        {
            var imageInfo = new ImageInfo("/images/profile.jpg");

            Assert.Equal("/images/profile.jpg", imageInfo.Path);
        }

        [Fact]
        public void Constructor_SetsLastModifiedToUtcNow()
        {
            var before = DateTime.UtcNow;
            var imageInfo = new ImageInfo("/images/profile.jpg");
            var after = DateTime.UtcNow;

            Assert.InRange(imageInfo.LastModified, before, after);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var imageInfo = new ImageInfo("/images/profile.jpg");

            Assert.Equal(0, imageInfo.Id);
            Assert.Null(imageInfo.UserId);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var modified = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var imageInfo = new ImageInfo("/images/profile.jpg")
            {
                Path = "/images/other.png",
                LastModified = modified
            };

            Assert.Equal("/images/other.png", imageInfo.Path);
            Assert.Equal(modified, imageInfo.LastModified);
        }
    }
}
