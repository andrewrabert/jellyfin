using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class IndexingKindTests
{
    [Theory]
    [InlineData(IndexingKind.PremiereDate, 0)]
    [InlineData(IndexingKind.ProductionYear, 1)]
    [InlineData(IndexingKind.CommunityRating, 2)]
    public void Member_HasExpectedValue(IndexingKind member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(3, Enum.GetValues<IndexingKind>().Length);
    }
}
