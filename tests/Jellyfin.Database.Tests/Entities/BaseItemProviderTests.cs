using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemProviderTests
    {
        [Fact]
        public void Properties_RoundTrip()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var provider = new BaseItemProvider
            {
                ItemId = item.Id,
                Item = item,
                ProviderId = "Imdb",
                ProviderValue = "tt0111161"
            };

            Assert.Equal(item.Id, provider.ItemId);
            Assert.Same(item, provider.Item);
            Assert.Equal("Imdb", provider.ProviderId);
            Assert.Equal("tt0111161", provider.ProviderValue);
        }

        [Fact]
        public void ItemId_DefaultsToEmptyGuid()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var provider = new BaseItemProvider
            {
                Item = item,
                ProviderId = "Tmdb",
                ProviderValue = "278"
            };

            Assert.Equal(Guid.Empty, provider.ItemId);
        }
    }
}
