using System;
using System.Linq;
using System.Threading;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.LocalMetadata.Parsers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.LocalMetadata.Tests.Parsers
{
    public class BoxSetXmlParserTests
    {
        private readonly BoxSetXmlParser _parser;

        public BoxSetXmlParserTests()
        {
            var providerManager = new Mock<IProviderManager>();
            providerManager.Setup(x => x.GetExternalIdInfos(It.IsAny<IHasProviderIds>()))
                .Returns(Enumerable.Empty<ExternalIdInfo>());

            _parser = new BoxSetXmlParser(new NullLogger<BoxSetXmlParser>(), providerManager.Object);
        }

        [Fact]
        public void Fetch_Valid_Success()
        {
            var result = new MetadataResult<BoxSet>()
            {
                Item = new BoxSet()
            };

            _parser.Fetch(result, "Test Data/collection.xml", CancellationToken.None);
            var item = result.Item;

            Assert.Equal("The Avengers Collection", item.Name);
            Assert.Equal("Marvel's The Avengers Collection", item.OriginalTitle);
            Assert.Equal("avengers collection", item.ForcedSortName);
            Assert.Equal("A collection of Avengers movies.", item.Overview);
            Assert.Equal("PG-13", item.OfficialRating);
            Assert.Equal("Family", item.CustomRating);
            Assert.Equal(82f, item.CriticRating);
            Assert.Equal(7.5f, item.CommunityRating);
            Assert.Equal(new DateTime(2012, 4, 25, 0, 0, 0, DateTimeKind.Utc), item.PremiereDate);
            Assert.Equal(2012, item.ProductionYear);
            Assert.Equal(TimeSpan.FromMinutes(142).Ticks, item.RunTimeTicks);
            Assert.Equal("en", item.PreferredMetadataLanguage);
            Assert.Equal("US", item.PreferredMetadataCountryCode);
            Assert.Equal("PremiereDate", item.DisplayOrder);
            Assert.Equal("Some assembly required.", item.Tagline);
            Assert.True(item.IsLocked);
            Assert.Equal(new[] { MetadataField.Name, MetadataField.Overview }, item.LockedFields);
            Assert.Equal(new[] { "Action", "Adventure" }, item.Genres);
            Assert.Equal(new[] { "Marvel Studios" }, item.Studios);
            Assert.Equal(new[] { "Superhero" }, item.Tags);
            Assert.Single(item.RemoteTrailers);
            Assert.Equal("https://www.youtube.com/watch?v=eOrNdBpGMv8", item.RemoteTrailers[0].Url);
            Assert.Equal("tt2345678", item.GetProviderId(MetadataProvider.Imdb));

            Assert.Equal(2, item.LinkedChildren.Length);
#pragma warning disable CS0618 // Type or member is obsolete - the parser reads the legacy Path property
            Assert.Equal("/movies/The Avengers (2012)", item.LinkedChildren[0].Path);
            Assert.Equal("/movies/Avengers Endgame (2019)", item.LinkedChildren[1].Path);
#pragma warning restore CS0618
            Assert.All(item.LinkedChildren, i => Assert.Equal(LinkedChildType.Manual, i.Type));
        }

        [Fact]
        public void Fetch_EmptyPath_ThrowsArgumentException()
        {
            var result = new MetadataResult<BoxSet>()
            {
                Item = new BoxSet()
            };

            Assert.Throws<ArgumentException>(() => _parser.Fetch(result, string.Empty, CancellationToken.None));
        }

        [Fact]
        public void Fetch_NullResult_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _parser.Fetch(null!, "Test Data/collection.xml", CancellationToken.None));
        }
    }
}
