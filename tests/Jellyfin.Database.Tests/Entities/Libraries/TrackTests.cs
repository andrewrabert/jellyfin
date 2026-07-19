using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class TrackTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var track = new Track(library);

            Assert.Same(library, track.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var track = new Track(new Library("Test Library", "/media"));

            Assert.NotNull(track.Releases);
            Assert.Empty(track.Releases);
            Assert.NotNull(track.TrackMetadata);
            Assert.Empty(track.TrackMetadata);
        }

        [Fact]
        public void TrackNumber_DefaultsToNullAndRoundTrips()
        {
            var track = new Track(new Library("Test Library", "/media"));

            Assert.Null(track.TrackNumber);

            track.TrackNumber = 11;
            Assert.Equal(11, track.TrackNumber);
        }

        [Fact]
        public void Track_ImplementsExpectedTypes()
        {
            var track = new Track(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(track);
            Assert.IsAssignableFrom<IHasReleases>(track);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Track(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
