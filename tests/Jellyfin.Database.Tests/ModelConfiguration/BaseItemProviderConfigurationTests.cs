using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class BaseItemProviderConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemProvider>();

        Assert.Equal(new[] { "ItemId", "ProviderId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_HasItemNavigationToBaseItem()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemProvider>();

        var navigation = entityType.FindNavigation("Item");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(BaseItemEntity), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public void Configure_CreatesProviderLookupIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemProvider>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "ProviderId", "ItemId", "ProviderValue");

        Assert.False(index.IsUnique);
    }
}
