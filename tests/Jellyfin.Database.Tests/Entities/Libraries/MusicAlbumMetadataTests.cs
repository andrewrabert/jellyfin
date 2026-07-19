using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MusicAlbumMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MusicAlbumMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MusicAlbumMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MusicAlbumMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MusicAlbumMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new MusicAlbumMetadata("The Album", "eng");

            Assert.Equal("The Album", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Ctor_InitializesEmptyLabels()
        {
            var metadata = new MusicAlbumMetadata("The Album", "eng");

            Assert.NotNull(metadata.Labels);
            Assert.Empty(metadata.Labels);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new MusicAlbumMetadata("The Album", "eng");

            Assert.Null(metadata.Barcode);
            Assert.Null(metadata.LabelNumber);
            Assert.Null(metadata.Country);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var metadata = new MusicAlbumMetadata("The Album", "eng")
            {
                Barcode = "0123456789012",
                LabelNumber = "CAT-001",
                Country = "US"
            };

            Assert.Equal("0123456789012", metadata.Barcode);
            Assert.Equal("CAT-001", metadata.LabelNumber);
            Assert.Equal("US", metadata.Country);
        }
    }
}
