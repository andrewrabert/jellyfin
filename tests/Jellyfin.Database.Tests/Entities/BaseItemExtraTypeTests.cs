using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemExtraTypeTests
    {
        [Theory]
        [InlineData(BaseItemExtraType.Unknown, 0)]
        [InlineData(BaseItemExtraType.Clip, 1)]
        [InlineData(BaseItemExtraType.Trailer, 2)]
        [InlineData(BaseItemExtraType.BehindTheScenes, 3)]
        [InlineData(BaseItemExtraType.DeletedScene, 4)]
        [InlineData(BaseItemExtraType.Interview, 5)]
        [InlineData(BaseItemExtraType.Scene, 6)]
        [InlineData(BaseItemExtraType.Sample, 7)]
        [InlineData(BaseItemExtraType.ThemeSong, 8)]
        [InlineData(BaseItemExtraType.ThemeVideo, 9)]
        [InlineData(BaseItemExtraType.Featurette, 10)]
        [InlineData(BaseItemExtraType.Short, 11)]
        public void Values_AreStable(BaseItemExtraType type, int expected)
        {
            Assert.Equal(expected, (int)type);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(12, Enum.GetValues<BaseItemExtraType>().Length);
        }
    }
}
