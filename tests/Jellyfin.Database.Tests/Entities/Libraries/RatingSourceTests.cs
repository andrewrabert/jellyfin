using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class RatingSourceTests
    {
        [Fact]
        public void Ctor_SetsMinimumAndMaximumValues()
        {
            var source = new RatingSource(1, 10);

            Assert.Equal(1, source.MinimumValue);
            Assert.Equal(10, source.MaximumValue);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var source = new RatingSource(0, 10);

            Assert.Null(source.Name);
            Assert.Null(source.Source);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var providerId = new MetadataProviderId("id-1", new MetadataProvider("Provider"));
            var source = new RatingSource(0, 10)
            {
                Name = "Critics",
                MinimumValue = 1,
                MaximumValue = 5,
                Source = providerId
            };

            Assert.Equal("Critics", source.Name);
            Assert.Equal(1, source.MinimumValue);
            Assert.Equal(5, source.MaximumValue);
            Assert.Same(providerId, source.Source);
        }

        [Fact]
        public void RatingSource_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new RatingSource(0, 10));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new RatingSource(0, 10);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
