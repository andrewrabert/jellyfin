using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class SeasonMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SeasonMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SeasonMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SeasonMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new SeasonMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new SeasonMetadata("Season 1", "eng");

            Assert.Equal("Season 1", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void Outline_DefaultsToNullAndRoundTrips()
        {
            var metadata = new SeasonMetadata("Season 1", "eng");

            Assert.Null(metadata.Outline);

            metadata.Outline = "Outline";
            Assert.Equal("Outline", metadata.Outline);
        }
    }
}
