using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class DisplayPreferencesTests
    {
        [Fact]
        public void Constructor_SetsProvidedValues()
        {
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var prefs = new DisplayPreferences(userId, itemId, "client");

            Assert.Equal(userId, prefs.UserId);
            Assert.Equal(itemId, prefs.ItemId);
            Assert.Equal("client", prefs.Client);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var prefs = new DisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client");

            Assert.Equal(0, prefs.Id);
            Assert.False(prefs.ShowSidebar);
            Assert.True(prefs.ShowBackdrop);
            Assert.Equal(30000, prefs.SkipForwardLength);
            Assert.Equal(10000, prefs.SkipBackwardLength);
            Assert.Equal(ScrollDirection.Horizontal, prefs.ScrollDirection);
            Assert.Equal(ChromecastVersion.Stable, prefs.ChromecastVersion);
            Assert.Null(prefs.IndexBy);
            Assert.False(prefs.EnableNextVideoInfoOverlay);
            Assert.Null(prefs.DashboardTheme);
            Assert.Null(prefs.TvHome);
            Assert.NotNull(prefs.HomeSections);
            Assert.Empty(prefs.HomeSections);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var prefs = new DisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client")
            {
                ShowSidebar = true,
                ShowBackdrop = false,
                ScrollDirection = ScrollDirection.Vertical,
                IndexBy = IndexingKind.ProductionYear,
                SkipForwardLength = 15000,
                SkipBackwardLength = 5000,
                ChromecastVersion = ChromecastVersion.Unstable,
                EnableNextVideoInfoOverlay = true,
                DashboardTheme = "dark",
                TvHome = "home"
            };

            Assert.True(prefs.ShowSidebar);
            Assert.False(prefs.ShowBackdrop);
            Assert.Equal(ScrollDirection.Vertical, prefs.ScrollDirection);
            Assert.Equal(IndexingKind.ProductionYear, prefs.IndexBy);
            Assert.Equal(15000, prefs.SkipForwardLength);
            Assert.Equal(5000, prefs.SkipBackwardLength);
            Assert.Equal(ChromecastVersion.Unstable, prefs.ChromecastVersion);
            Assert.True(prefs.EnableNextVideoInfoOverlay);
            Assert.Equal("dark", prefs.DashboardTheme);
            Assert.Equal("home", prefs.TvHome);
        }

        [Fact]
        public void HomeSections_CanBeAddedTo()
        {
            var prefs = new DisplayPreferences(Guid.NewGuid(), Guid.NewGuid(), "client");
            var section = new HomeSection { Order = 1, Type = HomeSectionType.LatestMedia };

            prefs.HomeSections.Add(section);

            Assert.Single(prefs.HomeSections, section);
        }
    }
}
