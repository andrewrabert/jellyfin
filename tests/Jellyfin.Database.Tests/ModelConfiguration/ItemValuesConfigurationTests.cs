using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class ItemValuesConfigurationTests
{
    [Fact]
    public void Configure_UsesItemValueIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValue>();

        Assert.Equal(new[] { "ItemValueId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Fact]
    public void Configure_CreatesTypeCleanValueIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValue>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "Type", "CleanValue");

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_CreatesUniqueTypeValueIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ItemValue>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "Type", "Value");

        Assert.True(index.IsUnique);
    }
}
