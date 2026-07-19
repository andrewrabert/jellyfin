using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class PhotoTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var photo = new Photo(library);

            Assert.Same(library, photo.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var photo = new Photo(new Library("Test Library", "/media"));

            Assert.NotNull(photo.PhotoMetadata);
            Assert.Empty(photo.PhotoMetadata);
            Assert.NotNull(photo.Releases);
            Assert.Empty(photo.Releases);
        }

        [Fact]
        public void Photo_ImplementsExpectedTypes()
        {
            var photo = new Photo(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(photo);
            Assert.IsAssignableFrom<IHasReleases>(photo);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Photo(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
