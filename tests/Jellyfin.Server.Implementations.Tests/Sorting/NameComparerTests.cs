using System;
using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class NameComparerTests
{
    private readonly NameComparer _cmp = new NameComparer();

    public static TheoryData<BaseItem?, BaseItem?> Compare_GivenNull_ThrowsArgumentNullException_TestData()
        => new()
        {
            { null, new Audio() },
            { new Audio(), null }
        };

    [Theory]
    [MemberData(nameof(Compare_GivenNull_ThrowsArgumentNullException_TestData))]
    public void Compare_GivenNull_ThrowsArgumentNullException(BaseItem? x, BaseItem? y)
    {
        Assert.Throws<ArgumentNullException>(() => _cmp.Compare(x, y));
    }

    [Theory]
    [InlineData("Alpha", "Beta", -1)]
    [InlineData("Beta", "Alpha", 1)]
    [InlineData("Alpha", "Alpha", 0)]
    [InlineData("alpha", "ALPHA", 0)]
    [InlineData("Alpha", "alphb", -1)]
    public void Compare_ValidNames_SortsExpected(string name1, string name2, int expected)
    {
        BaseItem x = new Audio
        {
            Name = name1
        };
        BaseItem y = new Audio
        {
            Name = name2
        };

        Assert.Equal(expected, Math.Sign(_cmp.Compare(x, y)));
        Assert.Equal(-expected, Math.Sign(_cmp.Compare(y, x)));
    }
}
