using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MusicAlbumTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var album = new MusicAlbum(library);

            Assert.Same(library, album.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var album = new MusicAlbum(new Library("Test Library", "/media"));

            Assert.NotNull(album.MusicAlbumMetadata);
            Assert.Empty(album.MusicAlbumMetadata);
            Assert.NotNull(album.Tracks);
            Assert.Empty(album.Tracks);
        }

        [Fact]
        public void MusicAlbum_IsLibraryItem()
        {
            Assert.IsAssignableFrom<LibraryItem>(new MusicAlbum(new Library("Test Library", "/media")));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new MusicAlbum(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
