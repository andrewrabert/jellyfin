using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MetadataProviderTests
    {
        [Fact]
        public void Ctor_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MetadataProvider(null!));
        }

        [Fact]
        public void Ctor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MetadataProvider(string.Empty));
        }

        [Fact]
        public void Ctor_SetsName()
        {
            var provider = new MetadataProvider("TheProvider");

            Assert.Equal("TheProvider", provider.Name);
        }

        [Fact]
        public void Name_RoundTrips()
        {
            var provider = new MetadataProvider("TheProvider")
            {
                Name = "OtherProvider"
            };

            Assert.Equal("OtherProvider", provider.Name);
        }

        [Fact]
        public void MetadataProvider_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new MetadataProvider("TheProvider"));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new MetadataProvider("TheProvider");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
