using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class SeasonTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var season = new Season(library);

            Assert.Same(library, season.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var season = new Season(new Library("Test Library", "/media"));

            Assert.NotNull(season.SeasonMetadata);
            Assert.Empty(season.SeasonMetadata);
            Assert.NotNull(season.Episodes);
            Assert.Empty(season.Episodes);
        }

        [Fact]
        public void SeasonNumber_DefaultsToNullAndRoundTrips()
        {
            var season = new Season(new Library("Test Library", "/media"));

            Assert.Null(season.SeasonNumber);

            season.SeasonNumber = 2;
            Assert.Equal(2, season.SeasonNumber);
        }

        [Fact]
        public void Season_IsLibraryItem()
        {
            Assert.IsAssignableFrom<LibraryItem>(new Season(new Library("Test Library", "/media")));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Season(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
