using System;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Model.Drawing;
using Xunit;

namespace Jellyfin.Drawing.Tests
{
    public class NullImageEncoderTests
    {
        private readonly NullImageEncoder _encoder = new NullImageEncoder();

        [Fact]
        public void Name_ReturnsNullImageEncoder()
        {
            Assert.Equal("Null Image Encoder", _encoder.Name);
        }

        [Fact]
        public void Capabilities_AreDisabled()
        {
            Assert.False(_encoder.SupportsImageCollageCreation);
            Assert.False(_encoder.SupportsImageEncoding);
        }

        [Theory]
        [InlineData("png")]
        [InlineData("jpeg")]
        [InlineData("jpg")]
        [InlineData("PNG")]
        public void SupportedInputFormats_SupportedFormat_ContainsFormat(string format)
        {
            Assert.Contains(format, _encoder.SupportedInputFormats);
        }

        [Theory]
        [InlineData("webp")]
        [InlineData("gif")]
        [InlineData("svg")]
        public void SupportedInputFormats_UnsupportedFormat_DoesNotContainFormat(string format)
        {
            Assert.DoesNotContain(format, _encoder.SupportedInputFormats);
        }

        [Fact]
        public void SupportedOutputFormats_ContainsOnlyJpgAndPng()
        {
            Assert.Contains(ImageFormat.Jpg, _encoder.SupportedOutputFormats);
            Assert.Contains(ImageFormat.Png, _encoder.SupportedOutputFormats);
            Assert.Equal(2, _encoder.SupportedOutputFormats.Count);
        }

        [Fact]
        public void UnsupportedOperations_ThrowNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => _encoder.GetImageSize("image.png"));
            Assert.Throws<NotImplementedException>(() => _encoder.GetImageBlurHash(4, 4, "image.png"));
            Assert.Throws<NotImplementedException>(() => _encoder.EncodeImage("in.png", DateTime.UtcNow, "out.png", false, null, 90, new ImageProcessingOptions(), ImageFormat.Png));
            Assert.Throws<NotImplementedException>(() => _encoder.CreateImageCollage(new ImageCollageOptions(), "library"));
            Assert.Throws<NotImplementedException>(() => _encoder.CreateSplashscreen(Array.Empty<string>(), Array.Empty<string>()));
            Assert.Throws<NotImplementedException>(() => _encoder.CreateTrickplayTile(new ImageCollageOptions(), 90, 320, null));
        }
    }
}
