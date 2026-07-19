using System;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ActivityLogTests
    {
        [Fact]
        public void Constructor_SetsRequiredProperties()
        {
            var userId = Guid.NewGuid();
            var log = new ActivityLog("name", "type", userId);

            Assert.Equal("name", log.Name);
            Assert.Equal("type", log.Type);
            Assert.Equal(userId, log.UserId);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var before = DateTime.UtcNow;
            var log = new ActivityLog("name", "type", Guid.NewGuid());
            var after = DateTime.UtcNow;

            Assert.Equal(0, log.Id);
            Assert.Equal(LogLevel.Information, log.LogSeverity);
            Assert.InRange(log.DateCreated, before, after);
            Assert.Null(log.Overview);
            Assert.Null(log.ShortOverview);
            Assert.Null(log.ItemId);
            Assert.Equal(0u, log.RowVersion);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ActivityLog(null!, "type", Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivityLog(string.Empty, "type", Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_NullType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ActivityLog("name", null!, Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_EmptyType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ActivityLog("name", string.Empty, Guid.NewGuid()));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var log = new ActivityLog("name", "type", Guid.NewGuid());

            log.OnSavingChanges();
            Assert.Equal(1u, log.RowVersion);

            log.OnSavingChanges();
            Assert.Equal(2u, log.RowVersion);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var log = new ActivityLog("name", "type", Guid.NewGuid())
            {
                Overview = "overview",
                ShortOverview = "short",
                ItemId = "item-id",
                LogSeverity = LogLevel.Critical
            };

            Assert.Equal("overview", log.Overview);
            Assert.Equal("short", log.ShortOverview);
            Assert.Equal("item-id", log.ItemId);
            Assert.Equal(LogLevel.Critical, log.LogSeverity);
        }
    }
}
