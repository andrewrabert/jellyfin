using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class PeopleConfigurationTests
{
    [Fact]
    public void Configure_UsesIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<People>();

        Assert.Equal(new[] { "Id" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_CreatesNameIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<People>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "Name");

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_HasBaseItemsCollectionNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<People>();

        var navigation = entityType.FindNavigation("BaseItems");

        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
        Assert.Equal(typeof(PeopleBaseItemMap), navigation.TargetEntityType.ClrType);
    }
}
