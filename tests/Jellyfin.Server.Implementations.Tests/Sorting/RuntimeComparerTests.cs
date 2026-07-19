using System;
using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class RuntimeComparerTests
{
    private readonly RuntimeComparer _cmp = new RuntimeComparer();

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
    [InlineData(null, null, 0)]
    [InlineData(null, 0L, 0)]
    [InlineData(null, 1L, -1)]
    [InlineData(1L, null, 1)]
    [InlineData(100L, 100L, 0)]
    [InlineData(50L, 100L, -1)]
    [InlineData(100L, 50L, 1)]
    public void Compare_ValidRuntimes_SortsExpected(long? ticks1, long? ticks2, int expected)
    {
        BaseItem x = new Audio
        {
            RunTimeTicks = ticks1
        };
        BaseItem y = new Audio
        {
            RunTimeTicks = ticks2
        };

        Assert.Equal(expected, _cmp.Compare(x, y));
        Assert.Equal(-expected, _cmp.Compare(y, x));
    }
}
