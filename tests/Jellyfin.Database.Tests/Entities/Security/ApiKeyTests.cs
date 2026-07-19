using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Security
{
    public class ApiKeyTests
    {
        [Fact]
        public void Ctor_SetsName()
        {
            var apiKey = new ApiKey("MyKey");

            Assert.Equal("MyKey", apiKey.Name);
        }

        [Fact]
        public void Ctor_GeneratesAccessToken()
        {
            var apiKey = new ApiKey("MyKey");

            Assert.Equal(32, apiKey.AccessToken.Length);
            Assert.True(Guid.TryParseExact(apiKey.AccessToken, "N", out _));
        }

        [Fact]
        public void Ctor_GeneratesUniqueAccessTokens()
        {
            var first = new ApiKey("MyKey");
            var second = new ApiKey("MyKey");

            Assert.NotEqual(first.AccessToken, second.AccessToken);
        }

        [Fact]
        public void Ctor_SetsDateCreated()
        {
            var before = DateTime.UtcNow;
            var apiKey = new ApiKey("MyKey");
            var after = DateTime.UtcNow;

            Assert.InRange(apiKey.DateCreated, before, after);
        }

        [Fact]
        public void DateLastActivity_DefaultsToMinValue()
        {
            var apiKey = new ApiKey("MyKey");

            Assert.Equal(default, apiKey.DateLastActivity);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var lastActivity = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
            var apiKey = new ApiKey("MyKey")
            {
                Name = "Renamed",
                AccessToken = "token",
                DateLastActivity = lastActivity
            };

            Assert.Equal("Renamed", apiKey.Name);
            Assert.Equal("token", apiKey.AccessToken);
            Assert.Equal(lastActivity, apiKey.DateLastActivity);
        }
    }
}
