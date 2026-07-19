using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class SeriesMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SeriesMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SeriesMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SeriesMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SeriesMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new SeriesMetadata("The Series", "eng");

            Assert.Equal("The Series", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Ctor_InitializesEmptyNetworks()
        {
            var metadata = new SeriesMetadata("The Series", "eng");

            Assert.NotNull(metadata.Networks);
            Assert.Empty(metadata.Networks);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new SeriesMetadata("The Series", "eng");

            Assert.Null(metadata.Outline);
            Assert.Null(metadata.Plot);
            Assert.Null(metadata.Tagline);
            Assert.Null(metadata.Country);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var metadata = new SeriesMetadata("The Series", "eng")
            {
                Outline = "Outline",
                Plot = "Plot",
                Tagline = "Tagline",
                Country = "US"
            };

            Assert.Equal("Outline", metadata.Outline);
            Assert.Equal("Plot", metadata.Plot);
            Assert.Equal("Tagline", metadata.Tagline);
            Assert.Equal("US", metadata.Country);
        }

        [Fact]
        public void Companies_ReturnsNetworks()
        {
            var metadata = new SeriesMetadata("The Series", "eng");

            Assert.Same(metadata.Networks, metadata.Companies);
            Assert.IsAssignableFrom<IHasCompanies>(metadata);
        }
    }
}
