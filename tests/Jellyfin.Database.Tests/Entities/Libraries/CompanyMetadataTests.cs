using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CompanyMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CompanyMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CompanyMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CompanyMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new CompanyMetadata("Studio", "eng");

            Assert.Equal("Studio", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var metadata = new CompanyMetadata("Studio", "eng");

            Assert.Null(metadata.Description);
            Assert.Null(metadata.Headquarters);
            Assert.Null(metadata.Country);
            Assert.Null(metadata.Homepage);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var metadata = new CompanyMetadata("Studio", "eng")
            {
                Description = "A studio.",
                Headquarters = "Hollywood",
                Country = "US",
                Homepage = "https://example.com"
            };

            Assert.Equal("A studio.", metadata.Description);
            Assert.Equal("Hollywood", metadata.Headquarters);
            Assert.Equal("US", metadata.Country);
            Assert.Equal("https://example.com", metadata.Homepage);
        }

        [Fact]
        public void CompanyMetadata_IsItemMetadata()
        {
            Assert.IsAssignableFrom<ItemMetadata>(new CompanyMetadata("Studio", "eng"));
        }
    }
}
