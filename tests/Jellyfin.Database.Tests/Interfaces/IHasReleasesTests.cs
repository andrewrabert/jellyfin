using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Interfaces;

public class IHasReleasesTests
{
    private static Movie CreateMovie()
        => new Movie(new Library("Movies", "/movies"));

    [Fact]
    public void Movie_ImplementsIHasReleases()
    {
        Assert.IsAssignableFrom<IHasReleases>(CreateMovie());
    }

    [Fact]
    public void Releases_StartsEmpty()
    {
        IHasReleases movie = CreateMovie();

        Assert.Empty(movie.Releases);
    }

    [Fact]
    public void Releases_CanAddItems()
    {
        IHasReleases movie = CreateMovie();
        var release = new Release("Director's Cut");

        movie.Releases.Add(release);

        Assert.Single(movie.Releases, release);
    }
}
