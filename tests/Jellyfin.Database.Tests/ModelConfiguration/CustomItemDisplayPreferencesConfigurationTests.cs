using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class CustomItemDisplayPreferencesConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueUserItemClientKeyIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<CustomItemDisplayPreferences>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "UserId", "ItemId", "Client", "Key");

        Assert.True(index.IsUnique);
    }
}
