using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class MediaFileKindTests
{
    [Theory]
    [InlineData(MediaFileKind.Main, 0)]
    [InlineData(MediaFileKind.Sidecar, 1)]
    [InlineData(MediaFileKind.AdditionalPart, 2)]
    [InlineData(MediaFileKind.AlternativeFormat, 3)]
    [InlineData(MediaFileKind.AdditionalStream, 4)]
    public void Member_HasExpectedValue(MediaFileKind member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(5, Enum.GetValues<MediaFileKind>().Length);
    }
}
