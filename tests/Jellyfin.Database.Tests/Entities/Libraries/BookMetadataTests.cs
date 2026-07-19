using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class BookMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BookMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BookMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BookMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new BookMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new BookMetadata("The Title", "eng");

            Assert.Equal("The Title", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Ctor_InitializesEmptyPublishers()
        {
            var metadata = new BookMetadata("The Title", "eng");

            Assert.NotNull(metadata.Publishers);
            Assert.Empty(metadata.Publishers);
        }

        [Fact]
        public void Isbn_DefaultsToNullAndRoundTrips()
        {
            var metadata = new BookMetadata("The Title", "eng");

            Assert.Null(metadata.Isbn);

            metadata.Isbn = 9783161484100;
            Assert.Equal(9783161484100, metadata.Isbn);
        }

        [Fact]
        public void Companies_ReturnsPublishers()
        {
            var metadata = new BookMetadata("The Title", "eng");

            Assert.Same(metadata.Publishers, metadata.Companies);
            Assert.IsAssignableFrom<IHasCompanies>(metadata);
        }
    }
}
