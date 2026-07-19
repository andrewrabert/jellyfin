using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class PersonTests
    {
        [Fact]
        public void Ctor_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Person(null!));
        }

        [Fact]
        public void Ctor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Person(string.Empty));
        }

        [Fact]
        public void Ctor_SetsName()
        {
            var person = new Person("John Doe");

            Assert.Equal("John Doe", person.Name);
        }

        [Fact]
        public void Ctor_SetsDateAddedAndDateModified()
        {
            var before = DateTime.UtcNow;
            var person = new Person("John Doe");
            var after = DateTime.UtcNow;

            Assert.InRange(person.DateAdded, before, after);
            Assert.Equal(person.DateAdded, person.DateModified);
        }

        [Fact]
        public void Ctor_InitializesEmptySources()
        {
            var person = new Person("John Doe");

            Assert.NotNull(person.Sources);
            Assert.Empty(person.Sources);
        }

        [Fact]
        public void SourceId_DefaultsToNullAndRoundTrips()
        {
            var person = new Person("John Doe");

            Assert.Null(person.SourceId);

            person.SourceId = "nm0000001";
            Assert.Equal("nm0000001", person.SourceId);
        }

        [Fact]
        public void Person_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Person("John Doe"));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Person("John Doe");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
