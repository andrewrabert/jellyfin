using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class IsFolderComparerTests
{
    private readonly IsFolderComparer _cmp = new IsFolderComparer();

    [Fact]
    public void Compare_FolderSortsBeforeNonFolder()
    {
        BaseItem folder = new Folder();
        BaseItem movie = new Movie();

        Assert.Equal(-1, _cmp.Compare(folder, movie));
        Assert.Equal(1, _cmp.Compare(movie, folder));
    }

    [Fact]
    public void Compare_SameKind_ReturnsZero()
    {
        Assert.Equal(0, _cmp.Compare(new Folder(), new Folder()));
        Assert.Equal(0, _cmp.Compare(new Movie(), new Movie()));
    }

    [Fact]
    public void Compare_GivenNull_TreatsAsFolder()
    {
        Assert.Equal(0, _cmp.Compare(null, null));
        Assert.Equal(0, _cmp.Compare(null, new Folder()));
        Assert.Equal(-1, _cmp.Compare(null, new Movie()));
        Assert.Equal(1, _cmp.Compare(new Movie(), null));
    }
}
