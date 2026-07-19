using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ItemDisplayPreferencesTests
    {
        [Fact]
        public void Constructor_SetsProvidedValues()
        {
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var prefs = new ItemDisplayPreferences(userId, itemId, "client");

            Assert.Equal(userId, prefs.UserId);
            Assert.Equal(itemId, prefs.ItemId);
            Assert.Equal("client", prefs.Client);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var prefs = new ItemDisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client");

            Assert.Equal(0, prefs.Id);
            Assert.Equal("SortName", prefs.SortBy);
            Assert.Equal(SortOrder.Ascending, prefs.SortOrder);
            Assert.False(prefs.RememberSorting);
            Assert.False(prefs.RememberIndexing);
            Assert.Null(prefs.IndexBy);
            Assert.Equal(ViewType.Albums, prefs.ViewType);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var prefs = new ItemDisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client")
            {
                ViewType = ViewType.Movies,
                RememberIndexing = true,
                IndexBy = IndexingKind.CommunityRating,
                RememberSorting = true,
                SortBy = "DateCreated",
                SortOrder = SortOrder.Descending
            };

            Assert.Equal(ViewType.Movies, prefs.ViewType);
            Assert.True(prefs.RememberIndexing);
            Assert.Equal(IndexingKind.CommunityRating, prefs.IndexBy);
            Assert.True(prefs.RememberSorting);
            Assert.Equal("DateCreated", prefs.SortBy);
            Assert.Equal(SortOrder.Descending, prefs.SortOrder);
        }
    }
}
