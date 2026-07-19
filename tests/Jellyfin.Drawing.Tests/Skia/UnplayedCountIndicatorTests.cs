using Jellyfin.Drawing.Skia;
using MediaBrowser.Model.Drawing;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Tests.Skia
{
    public class UnplayedCountIndicatorTests
    {
        [Theory]
        [InlineData(3)]
        [InlineData(42)]
        [InlineData(123)]
        public void DrawUnplayedCountIndicator_DrawsCircleInTopRightCorner(int count)
        {
            using var bitmap = new SKBitmap(200, 200);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            UnplayedCountIndicator.DrawUnplayedCountIndicator(canvas, new ImageDimensions(200, 200), count);

            // The indicator circle is centered at (width - 38, 38); sample a point inside it.
            var pixel = bitmap.GetPixel(162, 22);
            Assert.NotEqual(SKColors.White, pixel);
            Assert.True(pixel.Blue > pixel.Red);
        }

        [Fact]
        public void DrawUnplayedCountIndicator_DoesNotTouchOppositeCorner()
        {
            using var bitmap = new SKBitmap(200, 200);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            UnplayedCountIndicator.DrawUnplayedCountIndicator(canvas, new ImageDimensions(200, 200), 5);

            Assert.Equal(SKColors.White, bitmap.GetPixel(5, 195));
        }
    }
}
