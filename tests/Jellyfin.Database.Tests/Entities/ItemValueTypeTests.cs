using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ItemValueTypeTests
    {
        [Theory]
        [InlineData(ItemValueType.Artist, 0)]
        [InlineData(ItemValueType.AlbumArtist, 1)]
        [InlineData(ItemValueType.Genre, 2)]
        [InlineData(ItemValueType.Studios, 3)]
        [InlineData(ItemValueType.Tags, 4)]
        [InlineData(ItemValueType.InheritedTags, 6)]
        public void Values_AreStable(ItemValueType type, int expected)
        {
            Assert.Equal(expected, (int)type);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(6, Enum.GetValues<ItemValueType>().Length);
        }

        [Fact]
        public void Value5_IsNotDefined()
        {
            Assert.False(Enum.IsDefined((ItemValueType)5));
        }
    }
}
