using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class RatingTests
    {
        [Fact]
        public void Ctor_SetsValue()
        {
            var rating = new Rating(7.5);

            Assert.Equal(7.5, rating.Value);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var rating = new Rating(7.5);

            Assert.Null(rating.Votes);
            Assert.Null(rating.RatingType);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var source = new RatingSource(0, 10);
            var rating = new Rating(7.5)
            {
                Value = 9.1,
                Votes = 42,
                RatingType = source
            };

            Assert.Equal(9.1, rating.Value);
            Assert.Equal(42, rating.Votes);
            Assert.Same(source, rating.RatingType);
        }

        [Fact]
        public void Rating_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Rating(1));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Rating(1);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
