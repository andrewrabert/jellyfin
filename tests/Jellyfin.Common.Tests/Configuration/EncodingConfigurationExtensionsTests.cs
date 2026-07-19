using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Configuration;
using Moq;
using Xunit;

namespace Jellyfin.Common.Tests.Configuration
{
    public class EncodingConfigurationExtensionsTests
    {
        private readonly Mock<IConfigurationManager> _configurationManager;
        private readonly Mock<IApplicationPaths> _applicationPaths;
        private readonly EncodingOptions _encodingOptions;

        public EncodingConfigurationExtensionsTests()
        {
            _encodingOptions = new EncodingOptions();
            _applicationPaths = new Mock<IApplicationPaths>();
            _applicationPaths.Setup(p => p.CachePath).Returns(Path.Combine("data", "cache"));
            _configurationManager = new Mock<IConfigurationManager>();
            _configurationManager.Setup(c => c.GetConfiguration("encoding")).Returns(_encodingOptions);
            _configurationManager.Setup(c => c.CommonApplicationPaths).Returns(_applicationPaths.Object);
        }

        [Fact]
        public void GetEncodingOptions_Valid_Success()
        {
            Assert.Same(_encodingOptions, _configurationManager.Object.GetEncodingOptions());
        }

        [Fact]
        public void GetTranscodePath_NoCustomPath_DefaultsToCache()
        {
            _encodingOptions.TranscodingTempPath = null;

            var expected = Path.Combine("data", "cache", "transcodes");
            Assert.Equal(expected, _configurationManager.Object.GetTranscodePath());
            _applicationPaths.Verify(p => p.CreateAndCheckMarker(expected, "transcode", true), Times.Once);
        }

        [Fact]
        public void GetTranscodePath_CustomPath_Used()
        {
            var customPath = Path.Combine("custom", "transcodes");
            _encodingOptions.TranscodingTempPath = customPath;

            Assert.Equal(customPath, _configurationManager.Object.GetTranscodePath());
            _applicationPaths.Verify(p => p.CreateAndCheckMarker(customPath, "transcode", true), Times.Once);
        }
    }
}
