using System;
using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class AlbumComparerTests
{
    private readonly AlbumComparer _cmp = new AlbumComparer();

    [Theory]
    [InlineData("Abbey Road", "Abbey Road", 0)]
    [InlineData("abbey road", "ABBEY ROAD", 0)]
    [InlineData("Abbey Road", "Revolver", -1)]
    [InlineData("Revolver", "Abbey Road", 1)]
    public void Compare_AudioAlbums_SortsExpected(string album1, string album2, int expected)
    {
        BaseItem x = new Audio
        {
            Album = album1
        };
        BaseItem y = new Audio
        {
            Album = album2
        };

        Assert.Equal(expected, Math.Sign(_cmp.Compare(x, y)));
        Assert.Equal(-expected, Math.Sign(_cmp.Compare(y, x)));
    }

    [Fact]
    public void Compare_NonAudioItem_TreatedAsEmptyAlbum()
    {
        BaseItem x = new Movie();
        BaseItem y = new Audio
        {
            Album = "Abbey Road"
        };

        Assert.Equal(-1, Math.Sign(_cmp.Compare(x, y)));
        Assert.Equal(1, Math.Sign(_cmp.Compare(y, x)));
    }

    [Fact]
    public void Compare_GivenNull_TreatsAsEmptyAlbum()
    {
        Assert.Equal(0, _cmp.Compare(null, null));
        Assert.Equal(0, _cmp.Compare(null, new Movie()));
        Assert.Equal(1, Math.Sign(_cmp.Compare(new Audio { Album = "Abbey Road" }, null)));
    }
}
