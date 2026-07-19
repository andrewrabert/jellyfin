using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Providers;
using MediaBrowser.LocalMetadata.Images;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.MediaInfo;
using Moq;
using Xunit;

namespace Jellyfin.LocalMetadata.Tests.Images
{
    public sealed class LocalImageProviderTests : IDisposable
    {
        private readonly LocalImageProvider _provider;
        private readonly DirectoryInfo _tempDirectory;

        public LocalImageProviderTests()
        {
            var mediaSourceManager = new Mock<IMediaSourceManager>();
            mediaSourceManager.Setup(x => x.GetPathProtocol(It.IsAny<string>()))
                .Returns(MediaProtocol.File);
            BaseItem.MediaSourceManager = mediaSourceManager.Object;

            var fileSystem = new Mock<IFileSystem>();
            fileSystem.Setup(x => x.IsPathFile(It.IsAny<string>()))
                .Returns(true);
            BaseItem.FileSystem = fileSystem.Object;

            Video.RecordingsManager = new Mock<IRecordingsManager>().Object;

            _provider = new LocalImageProvider(new Mock<IFileSystem>().Object);
            _tempDirectory = Directory.CreateTempSubdirectory("jellyfin-localmetadata-tests");
        }

        [Fact]
        public void Supports_Movie_True()
        {
            Assert.True(_provider.Supports(new Movie { Path = "/movies/Some Movie/Some Movie.mkv" }));
        }

        [Fact]
        public void Supports_MusicAlbum_True()
        {
            Assert.True(_provider.Supports(new MusicAlbum { Path = "/music/Some Album" }));
        }

        [Fact]
        public void Supports_Episode_False()
        {
            Assert.False(_provider.Supports(new Episode { Path = "/shows/Some Show/Season 1/Episode 1.mkv" }));
        }

        [Fact]
        public void Supports_Audio_False()
        {
            Assert.False(_provider.Supports(new Audio { Path = "/music/Some Album/01 Track.flac" }));
        }

        [Fact]
        public void Supports_Photo_False()
        {
            Assert.False(_provider.Supports(new Photo { Path = "/photos/Vacation/photo.jpg" }));
        }

        [Fact]
        public void GetImages_MovieFolder_MapsFileNamesToImageTypes()
        {
            var movie = new Movie
            {
                Path = Path.Combine(_tempDirectory.FullName, "Some Movie.mkv")
            };

            var directoryService = CreateDirectoryService(
                _tempDirectory.FullName,
                "poster.jpg",
                "logo.png",
                "clearart.png",
                "disc.jpg",
                "banner.jpg",
                "landscape.jpg",
                "fanart.jpg");

            var images = _provider.GetImages(movie, directoryService).ToList();

            Assert.Equal(7, images.Count);
            AssertImage(images, ImageType.Primary, "poster.jpg");
            AssertImage(images, ImageType.Logo, "logo.png");
            AssertImage(images, ImageType.Art, "clearart.png");
            AssertImage(images, ImageType.Disc, "disc.jpg");
            AssertImage(images, ImageType.Banner, "banner.jpg");
            AssertImage(images, ImageType.Thumb, "landscape.jpg");
            AssertImage(images, ImageType.Backdrop, "fanart.jpg");
        }

        [Fact]
        public void GetImages_MovieNamedImage_MapsToPrimary()
        {
            var movie = new Movie
            {
                Path = Path.Combine(_tempDirectory.FullName, "Some Movie.mkv")
            };

            var directoryService = CreateDirectoryService(_tempDirectory.FullName, "Some Movie.jpg");

            var images = _provider.GetImages(movie, directoryService).ToList();

            var image = Assert.Single(images);
            Assert.Equal(ImageType.Primary, image.Type);
            Assert.Equal("Some Movie.jpg", Path.GetFileName(image.FileInfo.FullName));
        }

        [Fact]
        public void GetImages_MovieInMixedFolder_OnlyMatchesPrefixedImages()
        {
            var movie = new Movie
            {
                Path = Path.Combine(_tempDirectory.FullName, "Some Movie.mkv"),
                IsInMixedFolder = true
            };

            var directoryService = CreateDirectoryService(
                _tempDirectory.FullName,
                "Some Movie-poster.jpg",
                "poster.jpg");

            var images = _provider.GetImages(movie, directoryService).ToList();

            var image = Assert.Single(images);
            Assert.Equal(ImageType.Primary, image.Type);
            Assert.Equal("Some Movie-poster.jpg", Path.GetFileName(image.FileInfo.FullName));
        }

        [Fact]
        public void GetImages_MusicAlbum_PrefersFolderAndCdart()
        {
            var album = new MusicAlbum
            {
                Path = _tempDirectory.FullName
            };

            var directoryService = CreateDirectoryService(
                _tempDirectory.FullName,
                "folder.jpg",
                "cdart.jpg",
                "disc.jpg");

            var images = _provider.GetImages(album, directoryService).ToList();

            Assert.Equal(2, images.Count);
            AssertImage(images, ImageType.Primary, "folder.jpg");
            AssertImage(images, ImageType.Disc, "cdart.jpg");
        }

        [Fact]
        public void GetImages_NumberedBackdrops_AllAdded()
        {
            var movie = new Movie
            {
                Path = Path.Combine(_tempDirectory.FullName, "Some Movie.mkv")
            };

            var directoryService = CreateDirectoryService(
                _tempDirectory.FullName,
                "backdrop.jpg",
                "backdrop1.jpg",
                "backdrop2.jpg");

            var images = _provider.GetImages(movie, directoryService).ToList();

            Assert.Equal(3, images.Count);
            Assert.All(images, i => Assert.Equal(ImageType.Backdrop, i.Type));
        }

        public void Dispose()
        {
            _tempDirectory.Delete(true);
        }

        private static IDirectoryService CreateDirectoryService(string path, params string[] fileNames)
        {
            var entries = fileNames.Select(name => new FileSystemMetadata
            {
                FullName = Path.Combine(path, name),
                Name = name,
                Extension = Path.GetExtension(name),
                Length = 1,
                Exists = true
            }).ToArray();

            var directoryService = new Mock<IDirectoryService>();
            directoryService.Setup(x => x.GetFileSystemEntries(path))
                .Returns(entries);

            return directoryService.Object;
        }

        private static void AssertImage(IReadOnlyList<LocalImageInfo> images, ImageType type, string expectedFileName)
        {
            var image = Assert.Single(images, i => i.Type == type);
            Assert.Equal(expectedFileName, Path.GetFileName(image.FileInfo.FullName));
        }
    }
}
