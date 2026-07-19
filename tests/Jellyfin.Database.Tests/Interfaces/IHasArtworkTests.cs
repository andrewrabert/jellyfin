using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Interfaces;

public class IHasArtworkTests
{
    [Fact]
    public void ItemMetadata_ImplementsIHasArtwork()
    {
        var metadata = new MovieMetadata("Title", "eng");

        Assert.IsAssignableFrom<IHasArtwork>(metadata);
    }

    [Fact]
    public void Artwork_StartsEmpty()
    {
        IHasArtwork metadata = new MovieMetadata("Title", "eng");

        Assert.Empty(metadata.Artwork);
    }

    [Fact]
    public void Artwork_CanAddItems()
    {
        IHasArtwork metadata = new MovieMetadata("Title", "eng");
        var artwork = new Artwork("/some/path.jpg", ArtKind.Poster);

        metadata.Artwork.Add(artwork);

        Assert.Single(metadata.Artwork, artwork);
    }

    [Fact]
    public void PersonRole_ImplementsIHasArtwork()
    {
        Assert.True(typeof(IHasArtwork).IsAssignableFrom(typeof(PersonRole)));
    }
}
