using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class BookTests
    {
        [Fact]
        public void Ctor_SetsLibraryAndDateAdded()
        {
            var library = new Library("Test Library", "/media");
            var before = DateTime.UtcNow;
            var book = new Book(library);
            var after = DateTime.UtcNow;

            Assert.Same(library, book.Library);
            Assert.InRange(book.DateAdded, before, after);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var book = new Book(new Library("Test Library", "/media"));

            Assert.NotNull(book.BookMetadata);
            Assert.Empty(book.BookMetadata);
            Assert.NotNull(book.Releases);
            Assert.Empty(book.Releases);
        }

        [Fact]
        public void Book_ImplementsExpectedTypes()
        {
            var book = new Book(new Library("Test Library", "/media"));

            Assert.IsAssignableFrom<LibraryItem>(book);
            Assert.IsAssignableFrom<IHasReleases>(book);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Book(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
