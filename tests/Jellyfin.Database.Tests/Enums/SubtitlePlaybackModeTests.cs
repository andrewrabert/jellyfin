using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class SubtitlePlaybackModeTests
{
    [Theory]
    [InlineData(SubtitlePlaybackMode.Default, 0)]
    [InlineData(SubtitlePlaybackMode.Always, 1)]
    [InlineData(SubtitlePlaybackMode.OnlyForced, 2)]
    [InlineData(SubtitlePlaybackMode.None, 3)]
    [InlineData(SubtitlePlaybackMode.Smart, 4)]
    public void Member_HasExpectedValue(SubtitlePlaybackMode member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(5, Enum.GetValues<SubtitlePlaybackMode>().Length);
    }
}
