using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class PeopleBaseItemMapConfigurationTests
{
    public static TheoryData<string[]> ExpectedIndexes => new()
    {
        new[] { "ItemId", "SortOrder" },
        new[] { "ItemId", "ListOrder" },
        new[] { "PeopleId" }
    };

    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<PeopleBaseItemMap>();

        Assert.Equal(new[] { "ItemId", "PeopleId", "Role" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Theory]
    [MemberData(nameof(ExpectedIndexes))]
    public void Configure_CreatesExpectedIndex(string[] propertyNames)
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<PeopleBaseItemMap>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, propertyNames);

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_HasItemNavigationToBaseItem()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<PeopleBaseItemMap>();

        var navigation = entityType.FindNavigation("Item");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(BaseItemEntity), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public void Configure_HasPeopleNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<PeopleBaseItemMap>();

        var navigation = entityType.FindNavigation("People");

        Assert.NotNull(navigation);
        Assert.Equal(typeof(People), navigation.TargetEntityType.ClrType);
    }
}
