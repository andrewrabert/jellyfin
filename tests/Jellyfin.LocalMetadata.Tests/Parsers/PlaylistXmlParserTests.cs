using System;
using System.Linq;
using System.Threading;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.LocalMetadata.Parsers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.LocalMetadata.Tests.Parsers
{
    public class PlaylistXmlParserTests
    {
        private readonly PlaylistXmlParser _parser;

        public PlaylistXmlParserTests()
        {
            var providerManager = new Mock<IProviderManager>();
            providerManager.Setup(x => x.GetExternalIdInfos(It.IsAny<IHasProviderIds>()))
                .Returns(Enumerable.Empty<ExternalIdInfo>());

            _parser = new PlaylistXmlParser(new NullLogger<PlaylistXmlParser>(), providerManager.Object);
        }

        [Fact]
        public void Fetch_Valid_Success()
        {
            var result = new MetadataResult<Playlist>()
            {
                Item = new Playlist()
            };

            _parser.Fetch(result, "Test Data/playlist.xml", CancellationToken.None);
            var item = result.Item;

            Assert.Equal("Roadtrip Mix", item.Name);
            Assert.Equal(MediaType.Audio, item.PlaylistMediaType);
            Assert.Equal(Guid.Parse("44658d24-e31a-42f8-9de4-9b25f1f03e19"), item.OwnerUserId);

            // The share with an invalid user id is dropped
            var share = Assert.Single(item.Shares);
            Assert.Equal(Guid.Parse("e37a4674-9cfd-481c-8ab5-05b1f2d6ecbe"), share.UserId);
            Assert.True(share.CanEdit);

            // The empty PlaylistItem element is skipped
            Assert.Equal(2, item.LinkedChildren.Length);
#pragma warning disable CS0618 // Type or member is obsolete - the parser reads the legacy Path property
            Assert.Equal("/music/song one.mp3", item.LinkedChildren[0].Path);
            Assert.Equal("/music/song two.mp3", item.LinkedChildren[1].Path);
#pragma warning restore CS0618
        }

        [Fact]
        public void Fetch_EmptyPath_ThrowsArgumentException()
        {
            var result = new MetadataResult<Playlist>()
            {
                Item = new Playlist()
            };

            Assert.Throws<ArgumentException>(() => _parser.Fetch(result, string.Empty, CancellationToken.None));
        }
    }
}
