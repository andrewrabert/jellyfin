using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class CustomItemDisplayPreferencesTests
    {
        [Fact]
        public void Constructor_SetsAllProperties()
        {
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var prefs = new CustomItemDisplayPreferences(userId, itemId, "client", "key", "value");

            Assert.Equal(userId, prefs.UserId);
            Assert.Equal(itemId, prefs.ItemId);
            Assert.Equal("client", prefs.Client);
            Assert.Equal("key", prefs.Key);
            Assert.Equal("value", prefs.Value);
        }

        [Fact]
        public void Constructor_AllowsNullValue()
        {
            var prefs = new CustomItemDisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client", "key", null);

            Assert.Null(prefs.Value);
        }

        [Fact]
        public void Constructor_IdDefaultsToZero()
        {
            var prefs = new CustomItemDisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client", "key", "value");

            Assert.Equal(0, prefs.Id);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var prefs = new CustomItemDisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client", "key", null)
            {
                UserId = userId,
                ItemId = itemId,
                Client = "emby",
                Key = "other",
                Value = "42"
            };

            Assert.Equal(userId, prefs.UserId);
            Assert.Equal(itemId, prefs.ItemId);
            Assert.Equal("emby", prefs.Client);
            Assert.Equal("other", prefs.Key);
            Assert.Equal("42", prefs.Value);
        }
    }
}
