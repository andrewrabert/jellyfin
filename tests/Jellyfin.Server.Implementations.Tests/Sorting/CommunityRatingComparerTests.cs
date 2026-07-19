using System;
using Emby.Server.Implementations.Sorting;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Sorting;

public class CommunityRatingComparerTests
{
    private readonly CommunityRatingComparer _cmp = new CommunityRatingComparer();

    public static TheoryData<BaseItem?, BaseItem?> Compare_GivenNull_ThrowsArgumentNullException_TestData()
        => new()
        {
            { null, new Movie() },
            { new Movie(), null }
        };

    [Theory]
    [MemberData(nameof(Compare_GivenNull_ThrowsArgumentNullException_TestData))]
    public void Compare_GivenNull_ThrowsArgumentNullException(BaseItem? x, BaseItem? y)
    {
        Assert.Throws<ArgumentNullException>(() => _cmp.Compare(x, y));
    }

    [Theory]
    [InlineData(null, null, 0)]
    [InlineData(null, 0f, 0)]
    [InlineData(null, 7.5f, -1)]
    [InlineData(7.5f, null, 1)]
    [InlineData(7.5f, 7.5f, 0)]
    [InlineData(6.9f, 7f, -1)]
    [InlineData(7f, 6.9f, 1)]
    public void Compare_ValidRatings_SortsExpected(float? rating1, float? rating2, int expected)
    {
        BaseItem x = new Movie
        {
            CommunityRating = rating1
        };
        BaseItem y = new Movie
        {
            CommunityRating = rating2
        };

        Assert.Equal(expected, _cmp.Compare(x, y));
        Assert.Equal(-expected, _cmp.Compare(y, x));
    }
}
