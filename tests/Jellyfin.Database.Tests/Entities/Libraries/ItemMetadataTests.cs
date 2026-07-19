using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class ItemMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TestItemMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TestItemMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TestItemMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TestItemMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new TestItemMetadata("Title", "eng");

            Assert.Equal("Title", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Ctor_SetsDateAddedAndDateModified()
        {
            var before = DateTime.UtcNow;
            var metadata = new TestItemMetadata("Title", "eng");
            var after = DateTime.UtcNow;

            Assert.InRange(metadata.DateAdded, before, after);
            Assert.Equal(metadata.DateAdded, metadata.DateModified);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var metadata = new TestItemMetadata("Title", "eng");

            Assert.NotNull(metadata.PersonRoles);
            Assert.Empty(metadata.PersonRoles);
            Assert.NotNull(metadata.Genres);
            Assert.Empty(metadata.Genres);
            Assert.NotNull(metadata.Artwork);
            Assert.Empty(metadata.Artwork);
            Assert.NotNull(metadata.Ratings);
            Assert.Empty(metadata.Ratings);
            Assert.NotNull(metadata.Sources);
            Assert.Empty(metadata.Sources);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new TestItemMetadata("Title", "eng");

            Assert.Null(metadata.OriginalTitle);
            Assert.Null(metadata.OriginalLanguage);
            Assert.Null(metadata.SortTitle);
            Assert.Null(metadata.ReleaseDate);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var releaseDate = new DateTimeOffset(2020, 1, 2, 0, 0, 0, TimeSpan.Zero);
            var metadata = new TestItemMetadata("Title", "eng")
            {
                OriginalTitle = "Original",
                OriginalLanguage = "Deutsch",
                SortTitle = "Sortable",
                ReleaseDate = releaseDate
            };

            Assert.Equal("Original", metadata.OriginalTitle);
            Assert.Equal("Deutsch", metadata.OriginalLanguage);
            Assert.Equal("Sortable", metadata.SortTitle);
            Assert.Equal(releaseDate, metadata.ReleaseDate);
        }

        [Fact]
        public void ItemMetadata_ImplementsExpectedInterfaces()
        {
            var metadata = new TestItemMetadata("Title", "eng");

            Assert.IsAssignableFrom<IHasArtwork>(metadata);
            Assert.IsAssignableFrom<IHasConcurrencyToken>(metadata);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new TestItemMetadata("Title", "eng");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }

        private sealed class TestItemMetadata : ItemMetadata
        {
            public TestItemMetadata(string title, string language) : base(title, language)
            {
            }
        }
    }
}
