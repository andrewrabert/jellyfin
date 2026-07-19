using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MovieTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var movie = new Movie(library);

            Assert.Same(library, movie.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var movie = new Movie(new Library("Test Library", "/media"));

            Assert.NotNull(movie.Releases);
            Assert.Empty(movie.Releases);
            Assert.NotNull(movie.MovieMetadata);
            Assert.Empty(movie.MovieMetadata);
        }

        [Fact]
        public void Movie_ImplementsExpectedTypes()
        {
            var movie = new Movie(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(movie);
            Assert.IsAssignableFrom<IHasReleases>(movie);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Movie(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
