using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CollectionItemTests
    {
        private static LibraryItem CreateLibraryItem()
            => new Movie(new Library("Test Library", "/media"));

        [Fact]
        public void Ctor_SetsLibraryItem()
        {
            var libraryItem = CreateLibraryItem();
            var collectionItem = new CollectionItem(libraryItem);

            Assert.Same(libraryItem, collectionItem.LibraryItem);
        }

        [Fact]
        public void NextAndPrevious_DefaultToNullAndRoundTrip()
        {
            var collectionItem = new CollectionItem(CreateLibraryItem());

            Assert.Null(collectionItem.Next);
            Assert.Null(collectionItem.Previous);

            var next = new CollectionItem(CreateLibraryItem());
            var previous = new CollectionItem(CreateLibraryItem());
            collectionItem.Next = next;
            collectionItem.Previous = previous;

            Assert.Same(next, collectionItem.Next);
            Assert.Same(previous, collectionItem.Previous);
        }

        [Fact]
        public void CollectionItem_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new CollectionItem(CreateLibraryItem()));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new CollectionItem(CreateLibraryItem());

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
