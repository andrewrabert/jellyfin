using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class PersonRoleTests
    {
        [Fact]
        public void Ctor_SetsTypeAndPerson()
        {
            var person = new Person("John Doe");
            var role = new PersonRole(PersonRoleType.Actor, person);

            Assert.Equal(PersonRoleType.Actor, role.Type);
            Assert.Same(person, role.Person);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var role = new PersonRole(PersonRoleType.Actor, new Person("John Doe"));

            Assert.NotNull(role.Artwork);
            Assert.Empty(role.Artwork);
            Assert.NotNull(role.Sources);
            Assert.Empty(role.Sources);
        }

        [Fact]
        public void Role_DefaultsToNullAndRoundTrips()
        {
            var role = new PersonRole(PersonRoleType.Actor, new Person("John Doe"));

            Assert.Null(role.Role);

            role.Role = "Protagonist";
            Assert.Equal("Protagonist", role.Role);
        }

        [Fact]
        public void PersonRole_ImplementsExpectedInterfaces()
        {
            var role = new PersonRole(PersonRoleType.Actor, new Person("John Doe"));

            Assert.IsAssignableFrom<IHasArtwork>(role);
            Assert.IsAssignableFrom<IHasConcurrencyToken>(role);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new PersonRole(PersonRoleType.Actor, new Person("John Doe"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
