using System;
using Jellyfin.Database.Implementations.DbConfiguration;
using Xunit;

namespace Jellyfin.Database.Tests.DbConfiguration;

public class DatabaseLockingBehaviorTypesTests
{
    [Theory]
    [InlineData(DatabaseLockingBehaviorTypes.NoLock, 0)]
    [InlineData(DatabaseLockingBehaviorTypes.Pessimistic, 1)]
    [InlineData(DatabaseLockingBehaviorTypes.Optimistic, 2)]
    public void Member_HasExpectedValue(DatabaseLockingBehaviorTypes member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(3, Enum.GetValues<DatabaseLockingBehaviorTypes>().Length);
    }
}
