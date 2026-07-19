using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MetadataProviderIdTests
    {
        [Fact]
        public void Ctor_NullProviderId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MetadataProviderId(null!, new MetadataProvider("Provider")));
        }

        [Fact]
        public void Ctor_EmptyProviderId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MetadataProviderId(string.Empty, new MetadataProvider("Provider")));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsProperties()
        {
            var provider = new MetadataProvider("Provider");
            var providerId = new MetadataProviderId("tt0000001", provider);

            Assert.Equal("tt0000001", providerId.ProviderId);
            Assert.Same(provider, providerId.MetadataProvider);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var otherProvider = new MetadataProvider("Other");
            var providerId = new MetadataProviderId("tt0000001", new MetadataProvider("Provider"))
            {
                ProviderId = "tt0000002",
                MetadataProvider = otherProvider
            };

            Assert.Equal("tt0000002", providerId.ProviderId);
            Assert.Same(otherProvider, providerId.MetadataProvider);
        }

        [Fact]
        public void MetadataProviderId_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new MetadataProviderId("tt0000001", new MetadataProvider("Provider")));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new MetadataProviderId("tt0000001", new MetadataProvider("Provider"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
