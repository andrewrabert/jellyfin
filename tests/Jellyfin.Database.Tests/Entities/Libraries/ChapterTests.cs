using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class ChapterTests
    {
        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Chapter(null!, 0));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Chapter(string.Empty, 0));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsProperties()
        {
            var chapter = new Chapter("eng", 1234);

            Assert.Equal("eng", chapter.Language);
            Assert.Equal(1234, chapter.StartTime);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var chapter = new Chapter("eng", 0);

            Assert.Null(chapter.Name);
            Assert.Null(chapter.EndTime);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var chapter = new Chapter("eng", 0)
            {
                Name = "Chapter One",
                Language = "fra",
                StartTime = 10,
                EndTime = 20
            };

            Assert.Equal("Chapter One", chapter.Name);
            Assert.Equal("fra", chapter.Language);
            Assert.Equal(10, chapter.StartTime);
            Assert.Equal(20, chapter.EndTime);
        }

        [Fact]
        public void Chapter_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Chapter("eng", 0));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Chapter("eng", 0);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
