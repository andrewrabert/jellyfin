using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class ChapterConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<Chapter>();

        Assert.Equal(new[] { "ItemId", "ChapterIndex" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_HasItemNavigationToBaseItem()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<Chapter>();

        var navigation = entityType.FindNavigation("Item");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(BaseItemEntity), navigation.TargetEntityType.ClrType);
    }
}
