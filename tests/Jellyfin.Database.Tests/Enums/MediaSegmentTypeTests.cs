using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class MediaSegmentTypeTests
{
    [Theory]
    [InlineData(MediaSegmentType.Unknown, 0)]
    [InlineData(MediaSegmentType.Commercial, 1)]
    [InlineData(MediaSegmentType.Preview, 2)]
    [InlineData(MediaSegmentType.Recap, 3)]
    [InlineData(MediaSegmentType.Outro, 4)]
    [InlineData(MediaSegmentType.Intro, 5)]
    public void Member_HasExpectedValue(MediaSegmentType member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(6, Enum.GetValues<MediaSegmentType>().Length);
    }
}
