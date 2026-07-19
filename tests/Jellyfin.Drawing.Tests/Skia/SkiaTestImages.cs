using System;
using System.IO;
using SkiaSharp;

namespace Jellyfin.Drawing.Tests.Skia
{
    internal static class SkiaTestImages
    {
        internal static string CreateImage(string directory, int width, int height, SKColor color, SKEncodedImageFormat format = SKEncodedImageFormat.Png, string extension = ".png")
        {
            using var bitmap = new SKBitmap(width, height);
            bitmap.Erase(color);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, 100);
            var path = Path.Combine(directory, Guid.NewGuid().ToString("N") + extension);
            using var stream = File.Create(path);
            data.SaveTo(stream);
            return path;
        }

        internal static string CreateTempDirectory()
        {
            var path = Path.Combine(Path.GetTempPath(), "jellyfin-drawing-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
