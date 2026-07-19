using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MovieMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MovieMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovieMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MovieMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MovieMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new MovieMetadata("The Movie", "eng");

            Assert.Equal("The Movie", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Ctor_InitializesEmptyStudios()
        {
            var metadata = new MovieMetadata("The Movie", "eng");

            Assert.NotNull(metadata.Studios);
            Assert.Empty(metadata.Studios);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new MovieMetadata("The Movie", "eng");

            Assert.Null(metadata.Outline);
            Assert.Null(metadata.Tagline);
            Assert.Null(metadata.Plot);
            Assert.Null(metadata.Country);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var metadata = new MovieMetadata("The Movie", "eng")
            {
                Outline = "Outline",
                Tagline = "Tagline",
                Plot = "Plot",
                Country = "US"
            };

            Assert.Equal("Outline", metadata.Outline);
            Assert.Equal("Tagline", metadata.Tagline);
            Assert.Equal("Plot", metadata.Plot);
            Assert.Equal("US", metadata.Country);
        }

        [Fact]
        public void Companies_ReturnsStudios()
        {
            var metadata = new MovieMetadata("The Movie", "eng");

            Assert.Same(metadata.Studios, metadata.Companies);
            Assert.IsAssignableFrom<IHasCompanies>(metadata);
        }
    }
}
