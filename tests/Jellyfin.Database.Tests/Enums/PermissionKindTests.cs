using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class PermissionKindTests
{
    [Theory]
    [InlineData(PermissionKind.IsAdministrator, 0)]
    [InlineData(PermissionKind.IsHidden, 1)]
    [InlineData(PermissionKind.IsDisabled, 2)]
    [InlineData(PermissionKind.EnableSharedDeviceControl, 3)]
    [InlineData(PermissionKind.EnableRemoteAccess, 4)]
    [InlineData(PermissionKind.EnableLiveTvManagement, 5)]
    [InlineData(PermissionKind.EnableLiveTvAccess, 6)]
    [InlineData(PermissionKind.EnableMediaPlayback, 7)]
    [InlineData(PermissionKind.EnableAudioPlaybackTranscoding, 8)]
    [InlineData(PermissionKind.EnableVideoPlaybackTranscoding, 9)]
    [InlineData(PermissionKind.EnableContentDeletion, 10)]
    [InlineData(PermissionKind.EnableContentDownloading, 11)]
    [InlineData(PermissionKind.EnableSyncTranscoding, 12)]
    [InlineData(PermissionKind.EnableMediaConversion, 13)]
    [InlineData(PermissionKind.EnableAllDevices, 14)]
    [InlineData(PermissionKind.EnableAllChannels, 15)]
    [InlineData(PermissionKind.EnableAllFolders, 16)]
    [InlineData(PermissionKind.EnablePublicSharing, 17)]
    [InlineData(PermissionKind.EnableRemoteControlOfOtherUsers, 18)]
    [InlineData(PermissionKind.EnablePlaybackRemuxing, 19)]
    [InlineData(PermissionKind.ForceRemoteSourceTranscoding, 20)]
    [InlineData(PermissionKind.EnableCollectionManagement, 21)]
    [InlineData(PermissionKind.EnableSubtitleManagement, 22)]
    [InlineData(PermissionKind.EnableLyricManagement, 23)]
    public void Member_HasExpectedValue(PermissionKind member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(24, Enum.GetValues<PermissionKind>().Length);
    }
}
