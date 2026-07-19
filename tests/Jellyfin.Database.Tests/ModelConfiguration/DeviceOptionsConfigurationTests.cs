using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class DeviceOptionsConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueDeviceIdIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<DeviceOptions>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "DeviceId");

        Assert.True(index.IsUnique);
    }
}
