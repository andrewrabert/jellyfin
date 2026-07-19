using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ChapterTests
    {
        private static Chapter CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            return new Chapter
            {
                ItemId = item.Id,
                Item = item,
                ChapterIndex = 1,
                StartPositionTicks = 600_000_000L
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var chapter = CreateInstance();

            Assert.Equal(chapter.Item.Id, chapter.ItemId);
            Assert.Equal(1, chapter.ChapterIndex);
            Assert.Equal(600_000_000L, chapter.StartPositionTicks);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var chapter = CreateInstance();

            Assert.Null(chapter.Name);
            Assert.Null(chapter.ImagePath);
            Assert.Null(chapter.ImageDateModified);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var modified = DateTime.UtcNow;
            var chapter = CreateInstance();

            chapter.Name = "Chapter 1";
            chapter.ImagePath = "/chapters/1.jpg";
            chapter.ImageDateModified = modified;

            Assert.Equal("Chapter 1", chapter.Name);
            Assert.Equal("/chapters/1.jpg", chapter.ImagePath);
            Assert.Equal(modified, chapter.ImageDateModified);
        }
    }
}
