using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class PersonRoleTypeTests
{
    [Theory]
    [InlineData(PersonRoleType.Other, 0)]
    [InlineData(PersonRoleType.Director, 1)]
    [InlineData(PersonRoleType.Artist, 2)]
    [InlineData(PersonRoleType.OriginalArtist, 3)]
    [InlineData(PersonRoleType.Actor, 4)]
    [InlineData(PersonRoleType.VoiceActor, 5)]
    [InlineData(PersonRoleType.Producer, 6)]
    [InlineData(PersonRoleType.Remixer, 7)]
    [InlineData(PersonRoleType.Conductor, 8)]
    [InlineData(PersonRoleType.Composer, 9)]
    [InlineData(PersonRoleType.Author, 10)]
    [InlineData(PersonRoleType.Editor, 11)]
    public void Member_HasExpectedValue(PersonRoleType member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(12, Enum.GetValues<PersonRoleType>().Length);
    }
}
