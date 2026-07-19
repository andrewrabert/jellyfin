using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class EpisodeMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EpisodeMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new EpisodeMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new EpisodeMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new EpisodeMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new EpisodeMetadata("Pilot", "eng");

            Assert.Equal("Pilot", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new EpisodeMetadata("Pilot", "eng");

            Assert.Null(metadata.Outline);
            Assert.Null(metadata.Plot);
            Assert.Null(metadata.Tagline);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var metadata = new EpisodeMetadata("Pilot", "eng")
            {
                Outline = "Outline",
                Plot = "Plot",
                Tagline = "Tagline"
            };

            Assert.Equal("Outline", metadata.Outline);
            Assert.Equal("Plot", metadata.Plot);
            Assert.Equal("Tagline", metadata.Tagline);
        }
    }
}
