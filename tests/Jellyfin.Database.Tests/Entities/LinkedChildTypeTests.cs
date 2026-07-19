using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class LinkedChildTypeTests
    {
        [Theory]
        [InlineData(LinkedChildType.Manual, 0)]
        [InlineData(LinkedChildType.Shortcut, 1)]
        [InlineData(LinkedChildType.LocalAlternateVersion, 2)]
        [InlineData(LinkedChildType.LinkedAlternateVersion, 3)]
        public void Values_AreStable(LinkedChildType type, int expected)
        {
            Assert.Equal(expected, (int)type);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(4, Enum.GetValues<LinkedChildType>().Length);
        }
    }
}
