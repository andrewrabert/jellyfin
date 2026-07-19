using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CustomItemTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var item = new CustomItem(library);

            Assert.Same(library, item.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var item = new CustomItem(new Library("Test Library", "/media"));

            Assert.NotNull(item.CustomItemMetadata);
            Assert.Empty(item.CustomItemMetadata);
            Assert.NotNull(item.Releases);
            Assert.Empty(item.Releases);
        }

        [Fact]
        public void CustomItem_ImplementsExpectedTypes()
        {
            var item = new CustomItem(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(item);
            Assert.IsAssignableFrom<IHasReleases>(item);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new CustomItem(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
