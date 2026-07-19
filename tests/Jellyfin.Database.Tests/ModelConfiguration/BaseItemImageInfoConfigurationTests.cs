using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class BaseItemImageInfoConfigurationTests
{
    [Fact]
    public void Configure_UsesIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemImageInfo>();

        Assert.Equal(new[] { "Id" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_ItemForeignKey_UsesItemIdAndImagesNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemImageInfo>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ItemId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Item", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("Images", foreignKey.PrincipalToDependent?.Name);
    }

    [Fact]
    public void Configure_CreatesItemIdImageTypeIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemImageInfo>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "ItemId", "ImageType");

        Assert.False(index.IsUnique);
    }
}
