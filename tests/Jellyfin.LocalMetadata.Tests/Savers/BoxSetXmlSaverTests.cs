using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
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
    public sealed class BoxSetXmlSaverTests : IDisposable
    {
        private readonly BoxSetXmlSaver _saver;
        private readonly DirectoryInfo _tempDirectory;

        public BoxSetXmlSaverTests()
        {
            var mediaSourceManager = new Mock<IMediaSourceManager>();
            mediaSourceManager.Setup(x => x.GetPathProtocol(It.IsAny<string>()))
                .Returns(MediaProtocol.File);
            BaseItem.MediaSourceManager = mediaSourceManager.Object;
            Video.RecordingsManager = new Mock<IRecordingsManager>().Object;

            var configurationManager = new Mock<IServerConfigurationManager>();
            configurationManager.Setup(x => x.Configuration)
                .Returns(new ServerConfiguration());

            var libraryManager = new Mock<ILibraryManager>();
            libraryManager.Setup(x => x.GetPeople(It.IsAny<BaseItem>()))
                .Returns(Array.Empty<PersonInfo>());

            _saver = new BoxSetXmlSaver(
                new Mock<MediaBrowser.Model.IO.IFileSystem>().Object,
                configurationManager.Object,
                libraryManager.Object,
                new NullLogger<BoxSetXmlSaver>());

            _tempDirectory = Directory.CreateTempSubdirectory("jellyfin-localmetadata-tests");
        }

        [Fact]
        public void IsEnabledFor_BoxSetMetadataDownload_True()
        {
            var boxSet = new BoxSet { Path = "/collections/My Collection" };

            Assert.True(_saver.IsEnabledFor(boxSet, ItemUpdateType.MetadataDownload));
            Assert.True(_saver.IsEnabledFor(boxSet, ItemUpdateType.MetadataEdit));
        }

        [Fact]
        public void IsEnabledFor_BoxSetMetadataImport_False()
        {
            var boxSet = new BoxSet { Path = "/collections/My Collection" };

            Assert.False(_saver.IsEnabledFor(boxSet, ItemUpdateType.MetadataImport));
        }

        [Fact]
        public void IsEnabledFor_NonBoxSet_False()
        {
            var movie = new Movie { Path = "/movies/Some Movie/Some Movie.mkv" };

            Assert.False(_saver.IsEnabledFor(movie, ItemUpdateType.MetadataDownload));
        }

        [Fact]
        public void GetSavePath_BoxSet_CollectionXmlInItemFolder()
        {
            var boxSet = new BoxSet { Path = "/collections/My Collection" };

            Assert.Equal(Path.Combine("/collections/My Collection", "collection.xml"), _saver.GetSavePath(boxSet));
        }

        [Fact]
        public async Task SaveAsync_ParsedWithBoxSetXmlParser_RoundTrips()
        {
            var boxSet = new BoxSet
            {
                Path = _tempDirectory.FullName,
                Name = "The Avengers Collection",
                OriginalTitle = "Marvel's The Avengers Collection",
                ForcedSortName = "avengers collection",
                Overview = "A collection of Avengers movies.",
                OfficialRating = "PG-13",
                CommunityRating = 7.5f,
                ProductionYear = 2012,
                RunTimeTicks = TimeSpan.FromMinutes(142).Ticks,
                PreferredMetadataLanguage = "en",
                PreferredMetadataCountryCode = "US",
                Genres = new[] { "Action", "Adventure" },
                Studios = new[] { "Marvel Studios" },
                Tags = new[] { "Superhero" },
                IsLocked = true
            };
            boxSet.SetProviderId(MetadataProvider.Imdb, "tt2345678");

            await _saver.SaveAsync(boxSet, CancellationToken.None);

            var savedPath = Path.Combine(_tempDirectory.FullName, "collection.xml");
            Assert.True(File.Exists(savedPath));

            var imdbExternalIdInfo = new ExternalIdInfo("IMDb", MetadataProvider.Imdb.ToString(), null);
            var providerManager = new Mock<IProviderManager>();
            providerManager.Setup(x => x.GetExternalIdInfos(It.IsAny<IHasProviderIds>()))
                .Returns(new[] { imdbExternalIdInfo });
            var parser = new BoxSetXmlParser(new NullLogger<BoxSetXmlParser>(), providerManager.Object);

            var result = new MetadataResult<BoxSet>()
            {
                Item = new BoxSet()
            };
            parser.Fetch(result, savedPath, CancellationToken.None);
            var parsed = result.Item;

            Assert.Equal(boxSet.Name, parsed.Name);
            Assert.Equal(boxSet.OriginalTitle, parsed.OriginalTitle);
            Assert.Equal(boxSet.ForcedSortName, parsed.ForcedSortName);
            Assert.Equal(boxSet.Overview, parsed.Overview);
            Assert.Equal(boxSet.OfficialRating, parsed.OfficialRating);
            Assert.Equal(boxSet.CommunityRating, parsed.CommunityRating);
            Assert.Equal(boxSet.ProductionYear, parsed.ProductionYear);
            Assert.Equal(boxSet.RunTimeTicks, parsed.RunTimeTicks);
            Assert.Equal(boxSet.PreferredMetadataLanguage, parsed.PreferredMetadataLanguage);
            Assert.Equal(boxSet.PreferredMetadataCountryCode, parsed.PreferredMetadataCountryCode);
            Assert.Equal(boxSet.Genres, parsed.Genres);
            Assert.Equal(boxSet.Studios, parsed.Studios);
            Assert.Equal(boxSet.Tags, parsed.Tags);
            Assert.True(parsed.IsLocked);
            Assert.Equal("tt2345678", parsed.GetProviderId(MetadataProvider.Imdb));
        }

        public void Dispose()
        {
            _tempDirectory.Delete(true);
        }
    }
}
