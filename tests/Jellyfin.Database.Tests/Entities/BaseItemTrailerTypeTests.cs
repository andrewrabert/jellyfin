using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemTrailerTypeTests
    {
        [Fact]
        public void Properties_RoundTrip()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var trailerType = new BaseItemTrailerType
            {
                Id = 3,
                ItemId = item.Id,
                Item = item
            };

            Assert.Equal(3, trailerType.Id);
            Assert.Equal(item.Id, trailerType.ItemId);
            Assert.Same(item, trailerType.Item);
        }
    }
}
