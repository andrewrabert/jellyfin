using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class MediaFileTests
    {
        [Fact]
        public void Ctor_NullPath_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MediaFile(null!, MediaFileKind.Main));
        }

        [Fact]
        public void Ctor_EmptyPath_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new MediaFile(string.Empty, MediaFileKind.Main));
        }

        [Fact]
        public void Ctor_ValidArgs_SetsProperties()
        {
            var mediaFile = new MediaFile("movie.mkv", MediaFileKind.Main);

            Assert.Equal("movie.mkv", mediaFile.Path);
            Assert.Equal(MediaFileKind.Main, mediaFile.Kind);
        }

        [Fact]
        public void Ctor_InitializesEmptyMediaFileStreams()
        {
            var mediaFile = new MediaFile("movie.mkv", MediaFileKind.Main);

            Assert.NotNull(mediaFile.MediaFileStreams);
            Assert.Empty(mediaFile.MediaFileStreams);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var mediaFile = new MediaFile("movie.mkv", MediaFileKind.Main)
            {
                Path = "movie.part2.mkv",
                Kind = MediaFileKind.AdditionalPart
            };

            Assert.Equal("movie.part2.mkv", mediaFile.Path);
            Assert.Equal(MediaFileKind.AdditionalPart, mediaFile.Kind);
        }

        [Fact]
        public void MediaFile_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new MediaFile("movie.mkv", MediaFileKind.Main));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new MediaFile("movie.mkv", MediaFileKind.Main);

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
