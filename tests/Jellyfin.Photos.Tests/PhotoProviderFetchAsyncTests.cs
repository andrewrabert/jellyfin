using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Emby.Photos;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Photos.Tests
{
    public sealed class PhotoProviderFetchAsyncTests : IDisposable
    {
        private readonly Mock<IImageProcessor> _imageProcessor;
        private readonly PhotoProvider _provider;
        private readonly MetadataRefreshOptions _refreshOptions;
        private readonly DirectoryInfo _tempDirectory;

        public PhotoProviderFetchAsyncTests()
        {
            var mediaSourceManager = new Mock<IMediaSourceManager>();
            mediaSourceManager.Setup(x => x.GetPathProtocol(It.IsAny<string>()))
                .Returns<string>(path => path.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? MediaProtocol.Http : MediaProtocol.File);
            BaseItem.MediaSourceManager = mediaSourceManager.Object;

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(x => x.GetFileInfo(It.IsAny<string>()))
                .Returns<string>(path => new FileSystemMetadata { FullName = path, Exists = true });
            fileSystem.Setup(x => x.GetLastWriteTimeUtc(It.IsAny<FileSystemMetadata>()))
                .Returns(new DateTime(2022, 1, 15, 12, 0, 0, DateTimeKind.Utc));
            BaseItem.FileSystem = fileSystem.Object;

            _imageProcessor = new Mock<IImageProcessor>();
            _provider = new PhotoProvider(NullLogger<PhotoProvider>.Instance, _imageProcessor.Object);
            _refreshOptions = new MetadataRefreshOptions(Mock.Of<IDirectoryService>());
            _tempDirectory = Directory.CreateTempSubdirectory("jellyfin-photos-tests");
        }

        [Fact]
        public async Task FetchAsync_JpegWithMetadata_PopulatesItem()
        {
            var item = new Photo { Path = Path.Combine(AppContext.BaseDirectory, "Test Data", "jpeg-metadata.jpg") };

            var result = await _provider.FetchAsync(item, _refreshOptions, CancellationToken.None);

            Assert.Equal(ItemUpdateType.ImageUpdate | ItemUpdateType.MetadataImport, result);
            Assert.Equal(item.Path, item.GetImagePath(ImageType.Primary));
            Assert.Equal(10, item.Width);
            Assert.Equal(5, item.Height);
            Assert.Equal("Fujifilm", item.CameraMake);
            Assert.Equal("X100T", item.CameraModel);
            Assert.Equal("Darktable", item.Software);
            Assert.Equal(ImageOrientation.RightTop, item.Orientation);
            _imageProcessor.Verify(x => x.GetImageDimensions(It.IsAny<BaseItem>(), It.IsAny<ItemImageInfo>()), Times.Never);
        }

        [Fact]
        public async Task FetchAsync_JpegWithoutMetadata_PopulatesDimensionsOnly()
        {
            var item = new Photo { Path = Path.Combine(AppContext.BaseDirectory, "Test Data", "jpeg-no-metadata.jpg") };

            await _provider.FetchAsync(item, _refreshOptions, CancellationToken.None);

            Assert.Equal(8, item.Width);
            Assert.Equal(4, item.Height);
            Assert.True(string.IsNullOrEmpty(item.CameraMake));
            Assert.True(string.IsNullOrEmpty(item.CameraModel));
            Assert.Null(item.Orientation);
            _imageProcessor.Verify(x => x.GetImageDimensions(It.IsAny<BaseItem>(), It.IsAny<ItemImageInfo>()), Times.Never);
        }

        [Fact]
        public async Task FetchAsync_UnsupportedExtension_UsesImageProcessorDimensions()
        {
            var item = new Photo { Path = "/photos/photo.bmp" };
            _imageProcessor.Setup(x => x.GetImageDimensions(item, It.IsAny<ItemImageInfo>()))
                .Returns(new ImageDimensions(1920, 1080));

            var result = await _provider.FetchAsync(item, _refreshOptions, CancellationToken.None);

            Assert.Equal(ItemUpdateType.ImageUpdate | ItemUpdateType.MetadataImport, result);
            Assert.Equal(1920, item.Width);
            Assert.Equal(1080, item.Height);
        }

        [Fact]
        public async Task FetchAsync_UnsupportedImageFormat_LeavesDimensionsUnset()
        {
            var item = new Photo { Path = "/photos/photo.bmp" };
            _imageProcessor.Setup(x => x.GetImageDimensions(item, It.IsAny<ItemImageInfo>()))
                .Throws<ArgumentException>();

            var result = await _provider.FetchAsync(item, _refreshOptions, CancellationToken.None);

            Assert.Equal(ItemUpdateType.ImageUpdate | ItemUpdateType.MetadataImport, result);
            Assert.Equal(0, item.Width);
            Assert.Equal(0, item.Height);
        }

        [Fact]
        public async Task FetchAsync_CorruptFile_FallsBackToImageProcessor()
        {
            var path = Path.Combine(_tempDirectory.FullName, "corrupt.jpg");
            await File.WriteAllTextAsync(path, "not an image", TestContext.Current.CancellationToken);

            var item = new Photo { Path = path };
            _imageProcessor.Setup(x => x.GetImageDimensions(item, It.IsAny<ItemImageInfo>()))
                .Returns(new ImageDimensions(640, 480));

            var result = await _provider.FetchAsync(item, _refreshOptions, CancellationToken.None);

            Assert.Equal(ItemUpdateType.ImageUpdate | ItemUpdateType.MetadataImport, result);
            Assert.Equal(640, item.Width);
            Assert.Equal(480, item.Height);
        }

        public void Dispose()
        {
            _tempDirectory.Delete(true);
        }
    }
}
