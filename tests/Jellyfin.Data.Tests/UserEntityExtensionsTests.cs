using System;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Data.Tests
{
    public static class UserEntityExtensionsTests
    {
        private static User CreateUser()
            => new User("test-user", "auth-provider", "reset-provider");

        [Fact]
        public static void HasPermission_NotSet_ReturnsFalse()
        {
            var user = CreateUser();

            Assert.False(user.HasPermission(PermissionKind.IsAdministrator));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static void SetPermission_New_AddsPermission(bool value)
        {
            var user = CreateUser();

            user.SetPermission(PermissionKind.EnableMediaPlayback, value);

            Assert.Equal(value, user.HasPermission(PermissionKind.EnableMediaPlayback));
            Assert.Single(user.Permissions);
        }

        [Fact]
        public static void SetPermission_Existing_UpdatesValue()
        {
            var user = CreateUser();
            user.SetPermission(PermissionKind.IsAdministrator, false);

            user.SetPermission(PermissionKind.IsAdministrator, true);

            Assert.True(user.HasPermission(PermissionKind.IsAdministrator));
            Assert.Single(user.Permissions);
        }

        [Fact]
        public static void GetPreference_NotSet_ReturnsEmptyArray()
        {
            var user = CreateUser();

            Assert.Empty(user.GetPreference(PreferenceKind.BlockedTags));
        }

        [Fact]
        public static void SetPreference_RoundTrips()
        {
            var user = CreateUser();
            var values = new[] { "horror", "gore" };

            user.SetPreference(PreferenceKind.BlockedTags, values);

            Assert.Equal(values, user.GetPreference(PreferenceKind.BlockedTags));
        }

        [Fact]
        public static void SetPreference_Existing_Overwrites()
        {
            var user = CreateUser();
            user.SetPreference(PreferenceKind.BlockedTags, new[] { "old" });

            user.SetPreference(PreferenceKind.BlockedTags, new[] { "new" });

            Assert.Equal(new[] { "new" }, user.GetPreference(PreferenceKind.BlockedTags));
            Assert.Single(user.Preferences);
        }

        [Fact]
        public static void SetPreference_Generic_RoundTripsTypedValues()
        {
            var user = CreateUser();
            var values = new[] { 5, 10, 15 };

            user.SetPreference(PreferenceKind.MyMediaExcludes, values);

            Assert.Equal(values, user.GetPreferenceValues<int>(PreferenceKind.MyMediaExcludes));
        }

        [Fact]
        public static void GetPreferenceValues_NotSet_ReturnsEmptyArray()
        {
            var user = CreateUser();

            Assert.Empty(user.GetPreferenceValues<Guid>(PreferenceKind.GroupedFolders));
        }

        [Fact]
        public static void GetPreferenceValues_InvalidEntries_AreSkipped()
        {
            var user = CreateUser();
            var valid = Guid.NewGuid();
            user.SetPreference(PreferenceKind.GroupedFolders, new[] { valid.ToString("N"), "not-a-guid" });

            Assert.Equal(new[] { valid }, user.GetPreferenceValues<Guid>(PreferenceKind.GroupedFolders));
        }

        [Fact]
        public static void IsFolderGrouped_MatchesPreference()
        {
            var user = CreateUser();
            var grouped = Guid.NewGuid();
            user.SetPreference(PreferenceKind.GroupedFolders, new[] { grouped });

            Assert.True(user.IsFolderGrouped(grouped));
            Assert.False(user.IsFolderGrouped(Guid.NewGuid()));
        }

        [Fact]
        public static void AddDefaultPermissions_SetsExpectedValues()
        {
            var user = CreateUser();

            user.AddDefaultPermissions();

            Assert.False(user.HasPermission(PermissionKind.IsAdministrator));
            Assert.False(user.HasPermission(PermissionKind.IsDisabled));
            Assert.True(user.HasPermission(PermissionKind.IsHidden));
            Assert.True(user.HasPermission(PermissionKind.EnableMediaPlayback));
            Assert.True(user.HasPermission(PermissionKind.EnableRemoteAccess));
            Assert.False(user.HasPermission(PermissionKind.EnableRemoteControlOfOtherUsers));
        }

        [Fact]
        public static void AddDefaultPreferences_AddsEmptyPreferenceForEveryKind()
        {
            var user = CreateUser();

            user.AddDefaultPreferences();

            Assert.Equal(Enum.GetValues<PreferenceKind>().Length, user.Preferences.Count);
            Assert.All(user.Preferences, p => Assert.Equal(string.Empty, p.Value));
        }

        [Fact]
        public static void IsParentalScheduleAllowed_NoSchedules_ReturnsTrue()
        {
            var user = CreateUser();

            Assert.True(user.IsParentalScheduleAllowed());
        }

        [Fact]
        public static void IsParentalScheduleAllowed_EverydayAllHours_ReturnsTrue()
        {
            var user = CreateUser();
            user.AccessSchedules.Add(new AccessSchedule(DynamicDayOfWeek.Everyday, 0, 24, user.Id));

            Assert.True(user.IsParentalScheduleAllowed());
        }

        [Fact]
        public static void IsParentalScheduleAllowed_UnreachableWindow_ReturnsFalse()
        {
            var user = CreateUser();
            user.AccessSchedules.Add(new AccessSchedule(DynamicDayOfWeek.Everyday, 24, 24, user.Id));

            Assert.False(user.IsParentalScheduleAllowed());
        }
    }
}
