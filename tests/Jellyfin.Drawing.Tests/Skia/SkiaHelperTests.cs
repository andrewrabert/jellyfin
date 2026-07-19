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
    public sealed class SkiaHelperTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly SkiaEncoder _encoder;

        public SkiaHelperTests()
        {
            _tempDirectory = SkiaTestImages.CreateTempDirectory();

            var appPaths = new Mock<IApplicationPaths>();
            appPaths.Setup(p => p.TempDirectory).Returns(_tempDirectory);

            _encoder = new SkiaEncoder(NullLogger<SkiaEncoder>.Instance, appPaths.Object);
        }

        [Fact]
        public void GetNextValidImage_EmptyList_ReturnsNull()
        {
            var bitmap = SkiaHelper.GetNextValidImage(_encoder, Array.Empty<string>(), 0, out var newIndex);

            Assert.Null(bitmap);
            Assert.Equal(0, newIndex);
        }

        [Fact]
        public void GetNextValidImage_OnlyMissingFiles_ReturnsNull()
        {
            var paths = new[]
            {
                Path.Combine(_tempDirectory, "missing1.png"),
                Path.Combine(_tempDirectory, "missing2.png")
            };

            var bitmap = SkiaHelper.GetNextValidImage(_encoder, paths, 0, out _);

            Assert.Null(bitmap);
        }

        [Fact]
        public void GetNextValidImage_ValidImage_ReturnsBitmap()
        {
            var paths = new[] { SkiaTestImages.CreateImage(_tempDirectory, 3, 4, SKColors.Red) };

            using var bitmap = SkiaHelper.GetNextValidImage(_encoder, paths, 0, out var newIndex);

            Assert.NotNull(bitmap);
            Assert.Equal(3, bitmap.Width);
            Assert.Equal(4, bitmap.Height);
            Assert.Equal(1, newIndex);
        }

        [Fact]
        public void GetNextValidImage_SkipsMissingFile_ReturnsNextValidBitmap()
        {
            var paths = new[]
            {
                Path.Combine(_tempDirectory, "missing.png"),
                SkiaTestImages.CreateImage(_tempDirectory, 2, 2, SKColors.Green)
            };

            using var bitmap = SkiaHelper.GetNextValidImage(_encoder, paths, 0, out var newIndex);

            Assert.NotNull(bitmap);
            Assert.Equal(2, newIndex);
        }

        public void Dispose()
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
