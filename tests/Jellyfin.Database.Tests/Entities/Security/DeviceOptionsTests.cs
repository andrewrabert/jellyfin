using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Security
{
    public class DeviceOptionsTests
    {
        [Fact]
        public void Ctor_SetsDeviceId()
        {
            var options = new DeviceOptions("device-id");

            Assert.Equal("device-id", options.DeviceId);
        }

        [Fact]
        public void CustomName_DefaultsToNullAndRoundTrips()
        {
            var options = new DeviceOptions("device-id");

            Assert.Null(options.CustomName);

            options.CustomName = "Living Room TV";
            Assert.Equal("Living Room TV", options.CustomName);
        }

        [Fact]
        public void Id_DefaultsToZero()
        {
            var options = new DeviceOptions("device-id");

            Assert.Equal(0, options.Id);
        }
    }
}
