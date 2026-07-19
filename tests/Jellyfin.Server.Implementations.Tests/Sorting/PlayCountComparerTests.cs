using System;
using Emby.Server.Implementations.Sorting;
using Jellyfin.Database.Implementations.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class PlayCountComparerTests
{
    private readonly User _user = new User("test", "auth-provider", "reset-provider");

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(0, 1, -1)]
    [InlineData(1, 0, 1)]
    [InlineData(2, 10, -1)]
    public void Compare_PlayCounts_SortsExpected(int playCount1, int playCount2, int expected)
    {
        BaseItem x = new Movie { Id = Guid.NewGuid() };
        BaseItem y = new Movie { Id = Guid.NewGuid() };

        var userDataManager = new Mock<IUserDataManager>();
        userDataManager
            .Setup(m => m.GetUserData(_user, x))
            .Returns(new UserItemData { Key = "x", PlayCount = playCount1 });
        userDataManager
            .Setup(m => m.GetUserData(_user, y))
            .Returns(new UserItemData { Key = "y", PlayCount = playCount2 });

        var cmp = new PlayCountComparer
        {
            User = _user,
            UserDataManager = userDataManager.Object
        };

        Assert.Equal(expected, cmp.Compare(x, y));
        Assert.Equal(-expected, cmp.Compare(y, x));
    }

    [Fact]
    public void Compare_MissingUserData_TreatedAsZeroPlays()
    {
        BaseItem x = new Movie { Id = Guid.NewGuid() };
        BaseItem y = new Movie { Id = Guid.NewGuid() };

        var userDataManager = new Mock<IUserDataManager>();
        userDataManager
            .Setup(m => m.GetUserData(_user, x))
            .Returns((UserItemData?)null);
        userDataManager
            .Setup(m => m.GetUserData(_user, y))
            .Returns(new UserItemData { Key = "y", PlayCount = 5 });

        var cmp = new PlayCountComparer
        {
            User = _user,
            UserDataManager = userDataManager.Object
        };

        Assert.Equal(-1, cmp.Compare(x, y));
        Assert.Equal(1, cmp.Compare(y, x));
    }
}
