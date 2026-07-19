using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class PreferenceKindTests
{
    [Theory]
    [InlineData(PreferenceKind.BlockedTags, 0)]
    [InlineData(PreferenceKind.BlockedChannels, 1)]
    [InlineData(PreferenceKind.BlockedMediaFolders, 2)]
    [InlineData(PreferenceKind.EnabledDevices, 3)]
    [InlineData(PreferenceKind.EnabledChannels, 4)]
    [InlineData(PreferenceKind.EnabledFolders, 5)]
    [InlineData(PreferenceKind.EnableContentDeletionFromFolders, 6)]
    [InlineData(PreferenceKind.LatestItemExcludes, 7)]
    [InlineData(PreferenceKind.MyMediaExcludes, 8)]
    [InlineData(PreferenceKind.GroupedFolders, 9)]
    [InlineData(PreferenceKind.BlockUnratedItems, 10)]
    [InlineData(PreferenceKind.OrderedViews, 11)]
    [InlineData(PreferenceKind.AllowedTags, 12)]
    public void Member_HasExpectedValue(PreferenceKind member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(13, Enum.GetValues<PreferenceKind>().Length);
    }
}
