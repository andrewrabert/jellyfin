using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class GenreTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var genre = new Genre("Horror");

            Assert.Equal("Horror", genre.Name);
        }

        [Fact]
        public void Name_RoundTrips()
        {
            var genre = new Genre("Horror")
            {
                Name = "Comedy"
            };

            Assert.Equal("Comedy", genre.Name);
        }

        [Fact]
        public void Genre_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Genre("Horror"));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Genre("Horror");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
