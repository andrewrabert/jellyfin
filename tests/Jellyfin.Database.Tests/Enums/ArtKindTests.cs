using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class ArtKindTests
{
    [Theory]
    [InlineData(ArtKind.Other, 0)]
    [InlineData(ArtKind.Poster, 1)]
    [InlineData(ArtKind.Banner, 2)]
    [InlineData(ArtKind.Thumbnail, 3)]
    [InlineData(ArtKind.Logo, 4)]
    public void Member_HasExpectedValue(ArtKind member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(5, Enum.GetValues<ArtKind>().Length);
    }
}
