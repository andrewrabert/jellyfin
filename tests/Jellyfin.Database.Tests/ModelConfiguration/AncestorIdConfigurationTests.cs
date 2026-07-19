using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class AncestorIdConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<AncestorId>();

        Assert.Equal(new[] { "ItemId", "ParentItemId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_CreatesParentItemIdIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<AncestorId>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "ParentItemId");

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_ParentItemForeignKey_UsesParentItemIdAndChildrenNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<AncestorId>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ParentItemId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("ParentItem", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("Children", foreignKey.PrincipalToDependent?.Name);
    }

    [Fact]
    public void Configure_ItemForeignKey_UsesItemIdAndParentsNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<AncestorId>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ItemId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Item", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("Parents", foreignKey.PrincipalToDependent?.Name);
    }
}
