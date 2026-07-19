using System;
using System.IO;
using Jellyfin.Drawing.Skia;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Model.Drawing;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Tests.Skia
{
    public sealed class SkiaEncoderTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly SkiaEncoder _encoder;

        public SkiaEncoderTests()
        {
            _tempDirectory = SkiaTestImages.CreateTempDirectory();

            var appPaths = new Mock<IApplicationPaths>();
            appPaths.Setup(p => p.TempDirectory).Returns(_tempDirectory);
            appPaths.Setup(p => p.DataPath).Returns(_tempDirectory);

            _encoder = new SkiaEncoder(NullLogger<SkiaEncoder>.Instance, appPaths.Object);
        }

        [Fact]
        public void IsNativeLibAvailable_ReturnsTrue()
        {
            Assert.True(SkiaEncoder.IsNativeLibAvailable());
        }

        [Theory]
        [InlineData(ImageFormat.Bmp, SKEncodedImageFormat.Bmp)]
        [InlineData(ImageFormat.Jpg, SKEncodedImageFormat.Jpeg)]
        [InlineData(ImageFormat.Gif, SKEncodedImageFormat.Gif)]
        [InlineData(ImageFormat.Webp, SKEncodedImageFormat.Webp)]
        [InlineData(ImageFormat.Png, SKEncodedImageFormat.Png)]
        [InlineData(ImageFormat.Svg, SKEncodedImageFormat.Png)]
        public void GetImageFormat_ValidInput_ReturnsExpectedFormat(ImageFormat input, SKEncodedImageFormat expected)
        {
            Assert.Equal(expected, SkiaEncoder.GetImageFormat(input));
        }

        [Fact]
        public void GetImageSize_ValidImage_ReturnsDimensions()
        {
            var path = SkiaTestImages.CreateImage(_tempDirectory, 5, 7, SKColors.Red);

            var size = _encoder.GetImageSize(path);

            Assert.Equal(5, size.Width);
            Assert.Equal(7, size.Height);
        }

        [Fact]
        public void GetImageSize_MissingFile_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => _encoder.GetImageSize(Path.Combine(_tempDirectory, "missing.png")));
        }

        [Fact]
        public void GetImageSize_EmptyFile_ReturnsDefault()
        {
            var path = Path.Combine(_tempDirectory, "empty.jpg");
            File.Create(path).Dispose();

            var size = _encoder.GetImageSize(path);

            Assert.Equal(0, size.Width);
            Assert.Equal(0, size.Height);
        }

        [Fact]
        public void GetImageBlurHash_ValidImage_ReturnsHash()
        {
            var path = SkiaTestImages.CreateImage(_tempDirectory, 8, 8, SKColors.Green);

            var hash = _encoder.GetImageBlurHash(4, 4, path);

            Assert.NotEmpty(hash);
            Assert.Equal(hash, _encoder.GetImageBlurHash(4, 4, path));
        }

        [Theory]
        [InlineData("image.txt")]
        [InlineData("image.svg")]
        public void GetImageBlurHash_UnsupportedFormat_ReturnsEmpty(string fileName)
        {
            Assert.Equal(string.Empty, _encoder.GetImageBlurHash(4, 4, fileName));
        }

        [Fact]
        public void EncodeImage_UnsupportedInputFormat_ReturnsInputPath()
        {
            const string InputPath = "/media/image.txt";

            var result = _encoder.EncodeImage(InputPath, DateTime.UtcNow, "/out/image.png", false, null, 90, new ImageProcessingOptions(), ImageFormat.Png);

            Assert.Equal(InputPath, result);
        }

        [Fact]
        public void EncodeImage_SvgOutputFromRasterInput_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _encoder.EncodeImage("/media/image.png", DateTime.UtcNow, "/out/image.svg", false, null, 90, new ImageProcessingOptions(), ImageFormat.Svg));
        }

        [Fact]
        public void EncodeImage_Resize_WritesResizedImage()
        {
            var inputPath = SkiaTestImages.CreateImage(_tempDirectory, 2, 2, SKColors.Blue);
            var outputPath = Path.Combine(_tempDirectory, "resized.png");
            var options = new ImageProcessingOptions
            {
                Width = 4,
                Height = 4,
                Quality = 90,
                SupportedOutputFormats = new[] { ImageFormat.Png }
            };

            var result = _encoder.EncodeImage(inputPath, DateTime.UtcNow, outputPath, false, null, 90, options, ImageFormat.Png);

            Assert.Equal(outputPath, result);
            var size = _encoder.GetImageSize(outputPath);
            Assert.Equal(4, size.Width);
            Assert.Equal(4, size.Height);
        }

        public void Dispose()
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
