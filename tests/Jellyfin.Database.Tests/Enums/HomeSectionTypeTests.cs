using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class HomeSectionTypeTests
{
    [Theory]
    [InlineData(HomeSectionType.None, 0)]
    [InlineData(HomeSectionType.SmallLibraryTiles, 1)]
    [InlineData(HomeSectionType.LibraryButtons, 2)]
    [InlineData(HomeSectionType.ActiveRecordings, 3)]
    [InlineData(HomeSectionType.Resume, 4)]
    [InlineData(HomeSectionType.ResumeAudio, 5)]
    [InlineData(HomeSectionType.LatestMedia, 6)]
    [InlineData(HomeSectionType.NextUp, 7)]
    [InlineData(HomeSectionType.LiveTv, 8)]
    [InlineData(HomeSectionType.ResumeBook, 9)]
    public void Member_HasExpectedValue(HomeSectionType member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(10, Enum.GetValues<HomeSectionType>().Length);
    }
}
