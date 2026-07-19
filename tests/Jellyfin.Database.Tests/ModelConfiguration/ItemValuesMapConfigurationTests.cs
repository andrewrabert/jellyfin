using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class ItemValuesMapConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValueMap>();

        Assert.Equal(new[] { "ItemValueId", "ItemId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_HasItemNavigationToBaseItem()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValueMap>();

        var navigation = entityType.FindNavigation("Item");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(BaseItemEntity), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public void Configure_HasItemValueNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValueMap>();

        var navigation = entityType.FindNavigation("ItemValue");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(ItemValue), navigation.TargetEntityType.ClrType);
    }
}
