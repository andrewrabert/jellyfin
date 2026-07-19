using System;
using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class ProductionYearComparerTests
{
    private readonly ProductionYearComparer _cmp = new ProductionYearComparer();

    [Fact]
    public void Compare_GivenNull_TreatsAsZero()
    {
        Assert.Equal(0, _cmp.Compare(null, null));
        Assert.Equal(1, _cmp.Compare(new Movie { ProductionYear = 2000 }, null));
        Assert.Equal(-1, _cmp.Compare(null, new Movie { ProductionYear = 2000 }));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(2000, 2000, 0)]
    [InlineData(1999, 2000, -1)]
    [InlineData(2000, 1999, 1)]
    [InlineData(2000, null, 1)]
    [InlineData(null, 2000, -1)]
    public void Compare_ProductionYears_SortsExpected(int? year1, int? year2, int expected)
    {
        BaseItem x = new Movie
        {
            ProductionYear = year1
        };
        BaseItem y = new Movie
        {
            ProductionYear = year2
        };

        Assert.Equal(expected, Math.Sign(_cmp.Compare(x, y)));
        Assert.Equal(-expected, Math.Sign(_cmp.Compare(y, x)));
    }

    [Fact]
    public void Compare_MissingProductionYear_FallsBackToPremiereDateYear()
    {
        BaseItem x = new Movie
        {
            PremiereDate = new DateTime(1994, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        BaseItem y = new Movie
        {
            ProductionYear = 1994
        };

        Assert.Equal(0, _cmp.Compare(x, y));
    }

    [Fact]
    public void Compare_ProductionYearTakesPrecedenceOverPremiereDate()
    {
        BaseItem x = new Movie
        {
            ProductionYear = 1990,
            PremiereDate = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        BaseItem y = new Movie
        {
            ProductionYear = 2000
        };

        Assert.Equal(-1, Math.Sign(_cmp.Compare(x, y)));
    }
}
