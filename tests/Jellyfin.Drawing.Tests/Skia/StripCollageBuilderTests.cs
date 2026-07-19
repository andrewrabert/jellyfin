using System;
using System.IO;
using Jellyfin.Drawing.Skia;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Tests.Skia
{
    public sealed class StripCollageBuilderTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly SkiaEncoder _encoder;

        public StripCollageBuilderTests()
        {
            _tempDirectory = SkiaTestImages.CreateTempDirectory();

            var appPaths = new Mock<IApplicationPaths>();
            appPaths.Setup(p => p.TempDirectory).Returns(_tempDirectory);

            _encoder = new SkiaEncoder(NullLogger<SkiaEncoder>.Instance, appPaths.Object);
        }

        [Theory]
        [InlineData("collage.jpg", SKEncodedImageFormat.Jpeg)]
        [InlineData("collage.JPEG", SKEncodedImageFormat.Jpeg)]
        [InlineData("collage.webp", SKEncodedImageFormat.Webp)]
        [InlineData("collage.gif", SKEncodedImageFormat.Gif)]
        [InlineData("collage.bmp", SKEncodedImageFormat.Bmp)]
        [InlineData("collage.png", SKEncodedImageFormat.Png)]
        [InlineData("collage.unknown", SKEncodedImageFormat.Png)]
        public void GetEncodedFormat_ValidPath_ReturnsExpectedFormat(string outputPath, SKEncodedImageFormat expected)
        {
            Assert.Equal(expected, StripCollageBuilder.GetEncodedFormat(outputPath));
        }

        [Fact]
        public void GetEncodedFormat_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => StripCollageBuilder.GetEncodedFormat(null!));
        }

        [Fact]
        public void BuildSquareCollage_ValidImages_WritesCollageWithRequestedDimensions()
        {
            var paths = new[]
            {
                SkiaTestImages.CreateImage(_tempDirectory, 4, 4, SKColors.Red),
                SkiaTestImages.CreateImage(_tempDirectory, 4, 4, SKColors.Blue)
            };
            var outputPath = Path.Combine(_tempDirectory, "collage.png");

            new StripCollageBuilder(_encoder).BuildSquareCollage(paths, outputPath, 8, 8);

            using var collage = SKBitmap.Decode(outputPath);
            Assert.NotNull(collage);
            Assert.Equal(8, collage.Width);
            Assert.Equal(8, collage.Height);
        }

        public void Dispose()
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
