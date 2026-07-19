using Jellyfin.Drawing.Skia;
using MediaBrowser.Model.Drawing;
using SkiaSharp;
using Xunit;

namespace Jellyfin.Drawing.Tests.Skia
{
    public class PercentPlayedDrawerTests
    {
        private static readonly SKColor _accentColor = SKColor.Parse("#FF00A4DC");

        [Fact]
        public void Process_HalfPlayed_DrawsForegroundBarOnLeftOnly()
        {
            using var bitmap = new SKBitmap(100, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            PercentPlayedDrawer.Process(canvas, new ImageDimensions(100, 100), 50);

            Assert.Equal(_accentColor, bitmap.GetPixel(10, 95));
            Assert.NotEqual(_accentColor, bitmap.GetPixel(90, 95));
        }

        [Fact]
        public void Process_ZeroPercent_DrawsOnlyBackgroundBar()
        {
            using var bitmap = new SKBitmap(100, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            PercentPlayedDrawer.Process(canvas, new ImageDimensions(100, 100), 0);

            Assert.NotEqual(_accentColor, bitmap.GetPixel(10, 95));
            Assert.NotEqual(SKColors.White, bitmap.GetPixel(10, 95));
        }

        [Fact]
        public void Process_DoesNotTouchAreaAboveIndicator()
        {
            using var bitmap = new SKBitmap(100, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            PercentPlayedDrawer.Process(canvas, new ImageDimensions(100, 100), 100);

            Assert.Equal(SKColors.White, bitmap.GetPixel(50, 50));
        }
    }
}
