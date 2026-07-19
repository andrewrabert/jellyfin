using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class EpisodeTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var episode = new Episode(library);

            Assert.Same(library, episode.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var episode = new Episode(new Library("Test Library", "/media"));

            Assert.NotNull(episode.Releases);
            Assert.Empty(episode.Releases);
            Assert.NotNull(episode.EpisodeMetadata);
            Assert.Empty(episode.EpisodeMetadata);
        }

        [Fact]
        public void EpisodeNumber_DefaultsToNullAndRoundTrips()
        {
            var episode = new Episode(new Library("Test Library", "/media"));

            Assert.Null(episode.EpisodeNumber);

            episode.EpisodeNumber = 5;
            Assert.Equal(5, episode.EpisodeNumber);
        }

        [Fact]
        public void Episode_ImplementsExpectedTypes()
        {
            var episode = new Episode(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(episode);
            Assert.IsAssignableFrom<IHasReleases>(episode);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Episode(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
