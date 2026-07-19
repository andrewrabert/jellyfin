using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class DeviceConfigurationTests
{
    public static TheoryData<string[]> ExpectedIndexes => new()
    {
        new[] { "DeviceId", "DateLastActivity" },
        new[] { "AccessToken", "DateLastActivity" },
        new[] { "UserId", "DeviceId" }
    };

    [Theory]
    [MemberData(nameof(ExpectedIndexes))]
    public void Configure_CreatesExpectedIndex(string[] propertyNames)
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<Device>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, propertyNames);

        Assert.False(index.IsUnique);
    }
}
