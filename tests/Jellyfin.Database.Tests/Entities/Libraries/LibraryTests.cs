using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class LibraryTests
    {
        [Fact]
        public void Ctor_SetsNameAndPath()
        {
            var library = new Library("Movies", "/media/movies");

            Assert.Equal("Movies", library.Name);
            Assert.Equal("/media/movies", library.Path);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var library = new Library("Movies", "/media/movies")
            {
                Name = "Shows",
                Path = "/media/shows"
            };

            Assert.Equal("Shows", library.Name);
            Assert.Equal("/media/shows", library.Path);
        }

        [Fact]
        public void Library_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Library("Movies", "/media/movies"));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Library("Movies", "/media/movies");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
