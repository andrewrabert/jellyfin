using System;
using System.IO;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Drawing.Tests
{
    public sealed class ImageProcessorTests : IDisposable
    {
        private readonly ImageProcessor _imageProcessor;

        public ImageProcessorTests()
        {
            var config = new Mock<IServerConfigurationManager>();
            config.Setup(c => c.Configuration).Returns(new ServerConfiguration());

            _imageProcessor = new ImageProcessor(
                NullLogger<ImageProcessor>.Instance,
                Mock.Of<IServerApplicationPaths>(),
                Mock.Of<IFileSystem>(),
                new NullImageEncoder(),
                config.Object);
        }

        [Fact]
        public void GetImageCacheTag_SameInput_ReturnsSameTag()
        {
            var dateModified = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            var first = _imageProcessor.GetImageCacheTag("/media/movie/poster.png", dateModified);
            var second = _imageProcessor.GetImageCacheTag("/media/movie/poster.png", dateModified);

            Assert.Equal(first, second);
            Assert.Equal(32, first.Length);
        }

        [Fact]
        public void GetImageCacheTag_DifferentDate_ReturnsDifferentTag()
        {
            var dateModified = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            var first = _imageProcessor.GetImageCacheTag("/media/movie/poster.png", dateModified);
            var second = _imageProcessor.GetImageCacheTag("/media/movie/poster.png", dateModified.AddSeconds(1));

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void GetCachePath_ValidInput_ReturnsPrefixedPath()
        {
            var result = _imageProcessor.GetCachePath("/cache", "some-unique-name", ".png");

            var filename = Path.GetFileName(result);
            Assert.EndsWith(".png", result, StringComparison.Ordinal);
            Assert.Equal(Path.Join("/cache", filename[..1], filename), result);
        }

        [Fact]
        public void GetCachePath_SameUniqueName_IsDeterministic()
        {
            var first = _imageProcessor.GetCachePath("/cache", "some-unique-name", ".png");
            var second = _imageProcessor.GetCachePath("/cache", "some-unique-name", ".png");

            Assert.Equal(first, second);
        }

        [Theory]
        [InlineData("", "unique", ".png")]
        [InlineData("/cache", "", ".png")]
        [InlineData("/cache", "unique", "")]
        public void GetCachePath_EmptyInput_ThrowsArgumentException(string path, string uniqueName, string fileExtension)
        {
            Assert.Throws<ArgumentException>(() => _imageProcessor.GetCachePath(path, uniqueName, fileExtension));
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(100, 0)]
        [InlineData(-1, -1)]
        public void GetImageBlurHash_InvalidDimensions_ReturnsEmpty(int width, int height)
        {
            Assert.Equal(string.Empty, _imageProcessor.GetImageBlurHash("poster.png", new ImageDimensions(width, height)));
        }

        [Fact]
        public void SupportsImageCollageCreation_NullImageEncoder_ReturnsFalse()
        {
            Assert.False(_imageProcessor.SupportsImageCollageCreation);
        }

        [Fact]
        public void GetSupportedImageOutputFormats_NullImageEncoder_MatchesEncoder()
        {
            Assert.Equal(new NullImageEncoder().SupportedOutputFormats, _imageProcessor.GetSupportedImageOutputFormats());
        }

        public void Dispose()
        {
            _imageProcessor.Dispose();
        }
    }
}
