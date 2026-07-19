using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class UserTests
    {
        private static User CreateUser(string username = "user")
            => new User(username, "authProvider", "resetProvider");

        [Fact]
        public void Constructor_SetsProvidedValues()
        {
            var user = CreateUser("JohnDoe");

            Assert.Equal("JohnDoe", user.Username);
            Assert.Equal("authProvider", user.AuthenticationProviderId);
            Assert.Equal("resetProvider", user.PasswordResetProviderId);
        }

        [Fact]
        public void Constructor_SetsNormalizedUsernameToUppercase()
        {
            var user = CreateUser("JohnDoe");

            Assert.Equal("JOHNDOE", user.NormalizedUsername);
        }

        [Fact]
        public void Constructor_GeneratesNonEmptyId()
        {
            Assert.NotEqual(Guid.Empty, CreateUser().Id);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var user = CreateUser();

            Assert.Equal(0, user.InvalidLoginAttemptCount);
            Assert.True(user.EnableUserPreferenceAccess);
            Assert.False(user.MustUpdatePassword);
            Assert.False(user.DisplayMissingEpisodes);
            Assert.False(user.DisplayCollectionsView);
            Assert.True(user.HidePlayedInLatest);
            Assert.True(user.RememberAudioSelections);
            Assert.True(user.RememberSubtitleSelections);
            Assert.True(user.EnableNextEpisodeAutoPlay);
            Assert.False(user.EnableAutoLogin);
            Assert.True(user.PlayDefaultAudioTrack);
            Assert.Equal(SubtitlePlaybackMode.Default, user.SubtitleMode);
            Assert.Equal(SyncPlayUserAccessType.CreateAndJoinGroups, user.SyncPlayAccess);
            Assert.Null(user.Password);
            Assert.Null(user.AudioLanguagePreference);
            Assert.Null(user.SubtitleLanguagePreference);
            Assert.Null(user.LastActivityDate);
            Assert.Null(user.LastLoginDate);
            Assert.Null(user.LoginAttemptsBeforeLockout);
            Assert.Null(user.MaxParentalRatingScore);
            Assert.Null(user.MaxParentalRatingSubScore);
            Assert.Null(user.RemoteClientBitrateLimit);
            Assert.Null(user.ProfileImage);
            Assert.Null(user.CastReceiverId);
            Assert.Equal(0u, user.RowVersion);
        }

        [Fact]
        public void Constructor_InitializesEmptyCollections()
        {
            var user = CreateUser();

            Assert.Empty(user.AccessSchedules);
            Assert.Empty(user.DisplayPreferences);
            Assert.Empty(user.ItemDisplayPreferences);
            Assert.Empty(user.Permissions);
            Assert.Empty(user.Preferences);
        }

        [Fact]
        public void Constructor_NullUsername_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new User(null!, "auth", "reset"));
        }

        [Fact]
        public void Constructor_EmptyUsername_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User(string.Empty, "auth", "reset"));
        }

        [Fact]
        public void Constructor_NullAuthenticationProviderId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new User("user", null!, "reset"));
        }

        [Fact]
        public void Constructor_EmptyAuthenticationProviderId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User("user", string.Empty, "reset"));
        }

        [Fact]
        public void Constructor_NullPasswordResetProviderId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new User("user", "auth", null!));
        }

        [Fact]
        public void Constructor_EmptyPasswordResetProviderId_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new User("user", "auth", string.Empty));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var user = CreateUser();

            user.OnSavingChanges();
            Assert.Equal(1u, user.RowVersion);

            user.OnSavingChanges();
            Assert.Equal(2u, user.RowVersion);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var lastLogin = DateTime.UtcNow;
            var user = CreateUser();

            user.Password = "hash";
            user.MustUpdatePassword = true;
            user.AudioLanguagePreference = "eng";
            user.SubtitleLanguagePreference = "ger";
            user.InvalidLoginAttemptCount = 3;
            user.LastLoginDate = lastLogin;
            user.LoginAttemptsBeforeLockout = 5;
            user.MaxActiveSessions = 2;
            user.SubtitleMode = SubtitlePlaybackMode.Smart;
            user.RemoteClientBitrateLimit = 1_000_000;
            user.InternalId = 42L;
            user.SyncPlayAccess = SyncPlayUserAccessType.None;
            user.CastReceiverId = "receiver";

            Assert.Equal("hash", user.Password);
            Assert.True(user.MustUpdatePassword);
            Assert.Equal("eng", user.AudioLanguagePreference);
            Assert.Equal("ger", user.SubtitleLanguagePreference);
            Assert.Equal(3, user.InvalidLoginAttemptCount);
            Assert.Equal(lastLogin, user.LastLoginDate);
            Assert.Equal(5, user.LoginAttemptsBeforeLockout);
            Assert.Equal(2, user.MaxActiveSessions);
            Assert.Equal(SubtitlePlaybackMode.Smart, user.SubtitleMode);
            Assert.Equal(1_000_000, user.RemoteClientBitrateLimit);
            Assert.Equal(42L, user.InternalId);
            Assert.Equal(SyncPlayUserAccessType.None, user.SyncPlayAccess);
            Assert.Equal("receiver", user.CastReceiverId);
        }
    }
}
