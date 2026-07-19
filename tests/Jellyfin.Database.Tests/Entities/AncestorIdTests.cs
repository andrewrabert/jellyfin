using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class AncestorIdTests
    {
        [Fact]
        public void Properties_RoundTrip()
        {
            var parent = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Folder" };
            var child = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };

            var ancestorId = new AncestorId
            {
                ParentItemId = parent.Id,
                ItemId = child.Id,
                ParentItem = parent,
                Item = child
            };

            Assert.Equal(parent.Id, ancestorId.ParentItemId);
            Assert.Equal(child.Id, ancestorId.ItemId);
            Assert.Same(parent, ancestorId.ParentItem);
            Assert.Same(child, ancestorId.Item);
        }
    }
}
