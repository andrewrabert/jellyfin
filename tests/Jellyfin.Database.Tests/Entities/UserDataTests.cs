using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class UserDataTests
    {
        private static UserData CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            var user = new User("user", "auth", "reset");
            return new UserData
            {
                CustomDataKey = "key",
                ItemId = item.Id,
                Item = item,
                UserId = user.Id,
                User = user
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var userData = CreateInstance();

            Assert.Equal("key", userData.CustomDataKey);
            Assert.Equal(userData.Item!.Id, userData.ItemId);
            Assert.Equal(userData.User!.Id, userData.UserId);
        }

        [Fact]
        public void OptionalProperties_HaveExpectedDefaults()
        {
            var userData = CreateInstance();

            Assert.Null(userData.Rating);
            Assert.Equal(0L, userData.PlaybackPositionTicks);
            Assert.Equal(0, userData.PlayCount);
            Assert.False(userData.IsFavorite);
            Assert.Null(userData.LastPlayedDate);
            Assert.False(userData.Played);
            Assert.Null(userData.AudioStreamIndex);
            Assert.Null(userData.SubtitleStreamIndex);
            Assert.Null(userData.Likes);
            Assert.Null(userData.RetentionDate);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var lastPlayed = DateTime.UtcNow;
            var retention = lastPlayed.AddDays(30);
            var userData = CreateInstance();

            userData.Rating = 8.5;
            userData.PlaybackPositionTicks = 123L;
            userData.PlayCount = 4;
            userData.IsFavorite = true;
            userData.LastPlayedDate = lastPlayed;
            userData.Played = true;
            userData.AudioStreamIndex = 1;
            userData.SubtitleStreamIndex = 2;
            userData.Likes = true;
            userData.RetentionDate = retention;

            Assert.Equal(8.5, userData.Rating);
            Assert.Equal(123L, userData.PlaybackPositionTicks);
            Assert.Equal(4, userData.PlayCount);
            Assert.True(userData.IsFavorite);
            Assert.Equal(lastPlayed, userData.LastPlayedDate);
            Assert.True(userData.Played);
            Assert.Equal(1, userData.AudioStreamIndex);
            Assert.Equal(2, userData.SubtitleStreamIndex);
            Assert.True(userData.Likes);
            Assert.Equal(retention, userData.RetentionDate);
        }

        [Fact]
        public void NavigationProperties_AllowNull()
        {
            var userData = new UserData
            {
                CustomDataKey = "key",
                ItemId = Guid.NewGuid(),
                Item = null,
                UserId = Guid.NewGuid(),
                User = null
            };

            Assert.Null(userData.Item);
            Assert.Null(userData.User);
        }
    }
}
