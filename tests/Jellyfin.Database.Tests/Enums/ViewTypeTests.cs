using System;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Enums;

public class ViewTypeTests
{
    [Theory]
    [InlineData(ViewType.Albums, 0)]
    [InlineData(ViewType.AlbumArtists, 1)]
    [InlineData(ViewType.Artists, 2)]
    [InlineData(ViewType.Channels, 3)]
    [InlineData(ViewType.Collections, 4)]
    [InlineData(ViewType.Episodes, 5)]
    [InlineData(ViewType.Favorites, 6)]
    [InlineData(ViewType.Genres, 7)]
    [InlineData(ViewType.Guide, 8)]
    [InlineData(ViewType.Movies, 9)]
    [InlineData(ViewType.Networks, 10)]
    [InlineData(ViewType.Playlists, 11)]
    [InlineData(ViewType.Programs, 12)]
    [InlineData(ViewType.Recordings, 13)]
    [InlineData(ViewType.Schedule, 14)]
    [InlineData(ViewType.Series, 15)]
    [InlineData(ViewType.Shows, 16)]
    [InlineData(ViewType.Songs, 17)]
    [InlineData(ViewType.Suggestions, 18)]
    [InlineData(ViewType.Trailers, 19)]
    [InlineData(ViewType.Upcoming, 20)]
    [InlineData(ViewType.Authors, 21)]
    [InlineData(ViewType.Books, 22)]
    [InlineData(ViewType.Folders, 23)]
    [InlineData(ViewType.Mixed, 24)]
    [InlineData(ViewType.Photos, 25)]
    [InlineData(ViewType.PhotoAlbums, 26)]
    [InlineData(ViewType.SeriesTimers, 27)]
    [InlineData(ViewType.Studios, 28)]
    [InlineData(ViewType.Videos, 29)]
    public void Member_HasExpectedValue(ViewType member, int expected)
    {
        Assert.Equal(expected, (int)member);
    }

    [Fact]
    public void Enum_HasExpectedMemberCount()
    {
        Assert.Equal(30, Enum.GetValues<ViewType>().Length);
    }
}
