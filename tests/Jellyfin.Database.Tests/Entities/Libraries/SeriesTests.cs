using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class SeriesTests
    {
        [Fact]
        public void Ctor_SetsLibrary()
        {
            var library = new Library("Test Library", "/media");
            var series = new Series(library);

            Assert.Same(library, series.Library);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var series = new Series(new Library("Test Library", "/media"));

            Assert.NotNull(series.Seasons);
            Assert.Empty(series.Seasons);
            Assert.NotNull(series.SeriesMetadata);
            Assert.Empty(series.SeriesMetadata);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var series = new Series(new Library("Test Library", "/media"));

            Assert.Null(series.AirsDayOfWeek);
            Assert.Null(series.AirsTime);
            Assert.Null(series.FirstAired);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var airsTime = new DateTimeOffset(2020, 1, 1, 20, 30, 0, TimeSpan.Zero);
            var firstAired = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var series = new Series(new Library("Test Library", "/media"))
            {
                AirsDayOfWeek = DayOfWeek.Friday,
                AirsTime = airsTime,
                FirstAired = firstAired
            };

            Assert.Equal(DayOfWeek.Friday, series.AirsDayOfWeek);
            Assert.Equal(airsTime, series.AirsTime);
            Assert.Equal(firstAired, series.FirstAired);
        }

        [Fact]
        public void Series_IsLibraryItem()
        {
            Assert.IsAssignableFrom<LibraryItem>(new Series(new Library("Test Library", "/media")));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Series(new Library("Test Library", "/media"));

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
