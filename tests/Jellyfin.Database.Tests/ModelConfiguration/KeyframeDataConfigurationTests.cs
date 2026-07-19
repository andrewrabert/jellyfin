using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class KeyframeDataConfigurationTests
{
    [Fact]
    public void Configure_UsesItemIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<KeyframeData>();

        Assert.Equal(new[] { "ItemId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_ItemForeignKey_UsesItemId()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<KeyframeData>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ItemId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Item", foreignKey.DependentToPrincipal?.Name);
        Assert.Null(foreignKey.PrincipalToDependent);
    }
}
