using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CustomItemMetadataTests
    {
        [Fact]
        public void Ctor_NullTitle_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomItemMetadata(null!, "eng"));
        }

        [Fact]
        public void Ctor_EmptyTitle_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CustomItemMetadata(string.Empty, "eng"));
        }

        [Fact]
        public void Ctor_NullLanguage_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CustomItemMetadata("Title", null!));
        }

        [Fact]
        public void Ctor_EmptyLanguage_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new CustomItemMetadata("Title", string.Empty));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsTitleAndLanguage()
        {
            var metadata = new CustomItemMetadata("Item", "eng");

            Assert.Equal("Item", metadata.Title);
            Assert.Equal("eng", metadata.Language);
        }

        [Fact]
        public void CustomItemMetadata_IsItemMetadata()
        {
            Assert.IsAssignableFrom<ItemMetadata>(new CustomItemMetadata("Item", "eng"));
        }
    }
}
