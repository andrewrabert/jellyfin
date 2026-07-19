using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemMetadataFieldTests
    {
        [Fact]
        public void Properties_RoundTrip()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var field = new BaseItemMetadataField
            {
                Id = 7,
                ItemId = item.Id,
                Item = item
            };

            Assert.Equal(7, field.Id);
            Assert.Equal(item.Id, field.ItemId);
            Assert.Same(item, field.Item);
        }
    }
}
