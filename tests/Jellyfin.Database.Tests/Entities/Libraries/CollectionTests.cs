using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CollectionTests
    {
        [Fact]
        public void Ctor_InitializesEmptyItems()
        {
            var collection = new Collection();

            Assert.NotNull(collection.Items);
            Assert.Empty(collection.Items);
        }

        [Fact]
        public void Name_DefaultsToNullAndRoundTrips()
        {
            var collection = new Collection();

            Assert.Null(collection.Name);

            collection.Name = "Favorites";
            Assert.Equal("Favorites", collection.Name);
        }

        [Fact]
        public void Collection_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Collection());
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Collection();

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
