using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class ArtworkTests
    {
        [Fact]
        public void Ctor_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Artwork(null!, ArtKind.Poster));
        }

        [Fact]
        public void Ctor_EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Artwork(string.Empty, ArtKind.Poster));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsProperties()
        {
            var artwork = new Artwork("/art/poster.png", ArtKind.Poster);

            Assert.Equal("/art/poster.png", artwork.Path);
            Assert.Equal(ArtKind.Poster, artwork.Kind);
            Assert.Equal(0, artwork.Id);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var artwork = new Artwork("/art/poster.png", ArtKind.Poster)
            {
                Path = "/art/banner.png",
                Kind = ArtKind.Banner
            };

            Assert.Equal("/art/banner.png", artwork.Path);
            Assert.Equal(ArtKind.Banner, artwork.Kind);
        }

        [Fact]
        public void Artwork_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Artwork("/art.png", ArtKind.Other));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Artwork("/art.png", ArtKind.Other);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
