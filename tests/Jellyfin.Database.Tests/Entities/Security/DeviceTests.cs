using System;
using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Security
{
    public class DeviceTests
    {
        private static Device CreateDevice(Guid? userId = null)
            => new Device(
                userId ?? Guid.NewGuid(),
                "AppName",
                "1.0.0",
                "DeviceName",
                "device-id");

        [Fact]
        public void Ctor_SetsProperties()
        {
            var userId = Guid.NewGuid();
            var device = CreateDevice(userId);

            Assert.Equal(userId, device.UserId);
            Assert.Equal("AppName", device.AppName);
            Assert.Equal("1.0.0", device.AppVersion);
            Assert.Equal("DeviceName", device.DeviceName);
            Assert.Equal("device-id", device.DeviceId);
        }

        [Fact]
        public void Ctor_GeneratesAccessToken()
        {
            var device = CreateDevice();

            Assert.Equal(32, device.AccessToken.Length);
            Assert.True(Guid.TryParseExact(device.AccessToken, "N", out _));
        }

        [Fact]
        public void Ctor_SetsDates()
        {
            var before = DateTime.UtcNow;
            var device = CreateDevice();
            var after = DateTime.UtcNow;

            Assert.InRange(device.DateCreated, before, after);
            Assert.Equal(device.DateCreated, device.DateModified);
            Assert.Equal(device.DateCreated, device.DateLastActivity);
        }

        [Fact]
        public void IsActive_DefaultsToFalseAndRoundTrips()
        {
            var device = CreateDevice();

            Assert.False(device.IsActive);

            device.IsActive = true;
            Assert.True(device.IsActive);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var device = CreateDevice();
            var modified = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

            device.AccessToken = "token";
            device.AppName = "OtherApp";
            device.AppVersion = "2.0.0";
            device.DeviceName = "OtherDevice";
            device.DeviceId = "other-id";
            device.DateModified = modified;
            device.DateLastActivity = modified;

            Assert.Equal("token", device.AccessToken);
            Assert.Equal("OtherApp", device.AppName);
            Assert.Equal("2.0.0", device.AppVersion);
            Assert.Equal("OtherDevice", device.DeviceName);
            Assert.Equal("other-id", device.DeviceId);
            Assert.Equal(modified, device.DateModified);
            Assert.Equal(modified, device.DateLastActivity);
        }
    }
}
