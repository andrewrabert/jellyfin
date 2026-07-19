using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class LinkedChildEntityTests
    {
        private static LinkedChildEntity CreateInstance()
            => new LinkedChildEntity
            {
                ParentId = Guid.NewGuid(),
                ChildId = Guid.NewGuid(),
                ChildType = LinkedChildType.Manual
            };

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var entity = CreateInstance();

            Assert.NotEqual(Guid.Empty, entity.ParentId);
            Assert.NotEqual(Guid.Empty, entity.ChildId);
            Assert.Equal(LinkedChildType.Manual, entity.ChildType);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var entity = CreateInstance();

            Assert.Null(entity.SortOrder);
            Assert.Null(entity.Parent);
            Assert.Null(entity.Child);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var parent = new BaseItemEntity { Id = Guid.NewGuid(), Type = "BoxSet" };
            var child = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };

            var entity = new LinkedChildEntity
            {
                ParentId = parent.Id,
                ChildId = child.Id,
                ChildType = LinkedChildType.Shortcut,
                SortOrder = 4,
                Parent = parent,
                Child = child
            };

            Assert.Equal(parent.Id, entity.ParentId);
            Assert.Equal(child.Id, entity.ChildId);
            Assert.Equal(LinkedChildType.Shortcut, entity.ChildType);
            Assert.Equal(4, entity.SortOrder);
            Assert.Same(parent, entity.Parent);
            Assert.Same(child, entity.Child);
        }
    }
}
