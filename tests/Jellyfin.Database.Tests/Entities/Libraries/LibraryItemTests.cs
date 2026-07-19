using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class LibraryItemTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var item = new TestLibraryItem(library);

            Assert.Same(library, item.Library);
        }

        [Fact]
        public void Ctor_SetsDateAdded()
        {
            var before = DateTime.UtcNow;
            var item = new TestLibraryItem(new Library("Test Library", "/media"));
            var after = DateTime.UtcNow;

            Assert.InRange(item.DateAdded, before, after);
        }

        [Fact]
        public void Library_RoundTrips()
        {
            var otherLibrary = new Library("Other", "/other");
            var item = new TestLibraryItem(new Library("Test Library", "/media"))
            {
                Library = otherLibrary
            };

            Assert.Same(otherLibrary, item.Library);
        }

        [Fact]
        public void LibraryItem_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new TestLibraryItem(new Library("Test Library", "/media")));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new TestLibraryItem(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }

        private sealed class TestLibraryItem : LibraryItem
        {
            public TestLibraryItem(Library library) : base(library)
            {
            }
        }
    }
}
