using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MediaFileStreamTests
    {
        [Fact]
        public void Ctor_SetsStreamNumber()
        {
            var stream = new MediaFileStream(3);

            Assert.Equal(3, stream.StreamNumber);
        }

        [Fact]
        public void StreamNumber_RoundTrips()
        {
            var stream = new MediaFileStream(0)
            {
                StreamNumber = 7
            };

            Assert.Equal(7, stream.StreamNumber);
        }

        [Fact]
        public void MediaFileStream_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new MediaFileStream(0));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new MediaFileStream(0);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
