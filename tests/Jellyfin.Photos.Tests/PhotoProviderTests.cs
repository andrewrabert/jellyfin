using System;
using Emby.Photos;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Photos.Tests
{
    public class PhotoProviderTests
    {
        private static readonly DateTime _fileLastWriteTime = new DateTime(2022, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        private readonly PhotoProvider _provider;

        public PhotoProviderTests()
        {
            var mediaSourceManager = new Mock<IMediaSourceManager>();
            mediaSourceManager.Setup(x => x.GetPathProtocol(It.IsAny<string>()))
                .Returns<string>(path => path.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? MediaProtocol.Http : MediaProtocol.File);
            BaseItem.MediaSourceManager = mediaSourceManager.Object;

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(x => x.GetFileInfo(It.IsAny<string>()))
                .Returns<string>(path => new FileSystemMetadata { FullName = path, Exists = true });
            fileSystem.Setup(x => x.GetLastWriteTimeUtc(It.IsAny<FileSystemMetadata>()))
                .Returns(_fileLastWriteTime);
            BaseItem.FileSystem = fileSystem.Object;

            _provider = new PhotoProvider(NullLogger<PhotoProvider>.Instance, Mock.Of<IImageProcessor>());
        }

        [Fact]
        public void Name_ReturnsEmbeddedInformation()
        {
            Assert.Equal("Embedded Information", _provider.Name);
        }

        [Fact]
        public void HasChanged_NonFileProtocol_ReturnsFalse()
        {
            var item = new Photo { Path = "https://example.com/photo.jpg" };
            var directoryService = new Mock<IDirectoryService>(MockBehavior.Strict);

            Assert.False(_provider.HasChanged(item, directoryService.Object));
        }

        [Fact]
        public void HasChanged_FileNotFound_ReturnsFalse()
        {
            var item = new Photo { Path = "/photos/missing.jpg" };
            var directoryService = new Mock<IDirectoryService>();
            directoryService.Setup(x => x.GetFile(item.Path))
                .Returns((FileSystemMetadata?)null);

            Assert.False(_provider.HasChanged(item, directoryService.Object));
        }

        [Fact]
        public void HasChanged_FileModifiedAfterItem_ReturnsTrue()
        {
            var item = new Photo
            {
                Path = "/photos/holiday.jpg",
                DateModified = _fileLastWriteTime
            };

            var directoryService = new Mock<IDirectoryService>();
            directoryService.Setup(x => x.GetFile(item.Path))
                .Returns(new FileSystemMetadata
                {
                    FullName = item.Path,
                    Exists = true,
                    LastWriteTimeUtc = _fileLastWriteTime.AddHours(2)
                });

            Assert.True(_provider.HasChanged(item, directoryService.Object));
        }

        [Fact]
        public void HasChanged_FileNotModified_ReturnsFalse()
        {
            var item = new Photo
            {
                Path = "/photos/holiday.jpg",
                DateModified = _fileLastWriteTime
            };

            var directoryService = new Mock<IDirectoryService>();
            directoryService.Setup(x => x.GetFile(item.Path))
                .Returns(new FileSystemMetadata
                {
                    FullName = item.Path,
                    Exists = true,
                    LastWriteTimeUtc = _fileLastWriteTime
                });

            Assert.False(_provider.HasChanged(item, directoryService.Object));
        }
    }
}
