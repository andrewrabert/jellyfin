using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class SyncPlayUserAccessTypeTests
{
    [Theory]
    [InlineData(SyncPlayUserAccessType.CreateAndJoinGroups, 0)]
    [InlineData(SyncPlayUserAccessType.JoinGroups, 1)]
    [InlineData(SyncPlayUserAccessType.None, 2)]
    public void Member_HasExpectedValue(SyncPlayUserAccessType member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(3, Enum.GetValues<SyncPlayUserAccessType>().Length);
    }
}
