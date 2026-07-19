using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.LocalMetadata.Parsers;
using MediaBrowser.LocalMetadata.Savers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.LocalMetadata.Tests.Savers
{
    public sealed class PlaylistXmlSaverTests : IDisposable
    {
        private readonly PlaylistXmlSaver _saver;
        private readonly DirectoryInfo _tempDirectory;

        public PlaylistXmlSaverTests()
        {
            var mediaSourceManager = new Mock<IMediaSourceManager>();
            mediaSourceManager.Setup(x => x.GetPathProtocol(It.IsAny<string>()))
                .Returns(MediaProtocol.File);
            BaseItem.MediaSourceManager = mediaSourceManager.Object;

            var configurationManager = new Mock<IServerConfigurationManager>();
            configurationManager.Setup(x => x.Configuration)
                .Returns(new ServerConfiguration());

            var libraryManager = new Mock<ILibraryManager>();
            libraryManager.Setup(x => x.GetPeople(It.IsAny<BaseItem>()))
                .Returns(Array.Empty<PersonInfo>());

            _saver = new PlaylistXmlSaver(
                new Mock<MediaBrowser.Model.IO.IFileSystem>().Object,
                configurationManager.Object,
                libraryManager.Object,
                new NullLogger<PlaylistXmlSaver>());

            _tempDirectory = Directory.CreateTempSubdirectory("jellyfin-localmetadata-tests");
        }

        [Fact]
        public void IsEnabledFor_PlaylistMetadataImport_True()
        {
            var playlist = new Playlist { Path = "/playlists/Roadtrip Mix" };

            Assert.True(_saver.IsEnabledFor(playlist, ItemUpdateType.MetadataImport));
            Assert.True(_saver.IsEnabledFor(playlist, ItemUpdateType.MetadataEdit));
        }

        [Fact]
        public void IsEnabledFor_PlaylistNoUpdate_False()
        {
            var playlist = new Playlist { Path = "/playlists/Roadtrip Mix" };

            Assert.False(_saver.IsEnabledFor(playlist, ItemUpdateType.None));
        }

        [Fact]
        public void IsEnabledFor_NonPlaylist_False()
        {
            var boxSet = new BoxSet { Path = "/collections/My Collection" };

            Assert.False(_saver.IsEnabledFor(boxSet, ItemUpdateType.MetadataImport));
        }

        [Fact]
        public void GetSavePath_PlaylistFile_ChangesExtensionToXml()
        {
            Assert.Equal("/playlists/Roadtrip Mix.xml", PlaylistXmlSaver.GetSavePath("/playlists/Roadtrip Mix.m3u"));
        }

        [Fact]
        public void GetSavePath_PlaylistFolder_AppendsDefaultFilename()
        {
            Assert.Equal(Path.Combine("/playlists/Roadtrip Mix", "playlist.xml"), PlaylistXmlSaver.GetSavePath("/playlists/Roadtrip Mix"));
        }

        [Fact]
        public async Task SaveAsync_ParsedWithPlaylistXmlParser_RoundTrips()
        {
            var ownerUserId = Guid.Parse("44658d24-e31a-42f8-9de4-9b25f1f03e19");
            var shareUserId = Guid.Parse("e37a4674-9cfd-481c-8ab5-05b1f2d6ecbe");
            var playlist = new Playlist
            {
                Path = _tempDirectory.FullName,
                Name = "Roadtrip Mix",
                PlaylistMediaType = MediaType.Audio,
                OwnerUserId = ownerUserId,
                Shares = new[] { new PlaylistUserPermissions(shareUserId, true) }
            };

            await _saver.SaveAsync(playlist, CancellationToken.None);

            var savedPath = Path.Combine(_tempDirectory.FullName, "playlist.xml");
            Assert.True(File.Exists(savedPath));

            var providerManager = new Mock<IProviderManager>();
            providerManager.Setup(x => x.GetExternalIdInfos(It.IsAny<IHasProviderIds>()))
                .Returns(Enumerable.Empty<ExternalIdInfo>());
            var parser = new PlaylistXmlParser(new NullLogger<PlaylistXmlParser>(), providerManager.Object);

            var result = new MetadataResult<Playlist>()
            {
                Item = new Playlist()
            };
            parser.Fetch(result, savedPath, CancellationToken.None);
            var parsed = result.Item;

            Assert.Equal("Roadtrip Mix", parsed.Name);
            Assert.Equal(MediaType.Audio, parsed.PlaylistMediaType);
            Assert.Equal(ownerUserId, parsed.OwnerUserId);
            var share = Assert.Single(parsed.Shares);
            Assert.Equal(shareUserId, share.UserId);
            Assert.True(share.CanEdit);
        }

        public void Dispose()
        {
            _tempDirectory.Delete(true);
        }
    }
}
