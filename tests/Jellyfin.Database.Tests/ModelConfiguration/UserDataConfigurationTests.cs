using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class UserDataConfigurationTests
{
    public static TheoryData<string[]> ExpectedIndexes => new()
    {
        new[] { "ItemId", "UserId", "Played" },
        new[] { "ItemId", "UserId", "PlaybackPositionTicks" },
        new[] { "ItemId", "UserId", "IsFavorite" },
        new[] { "ItemId", "UserId", "LastPlayedDate" },
        new[] { "UserId", "ItemId", "LastPlayedDate" },
        new[] { "UserId", "Played", "ItemId" },
        new[] { "UserId", "IsFavorite", "ItemId" }
    };

    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<UserData>();

        Assert.Equal(new[] { "ItemId", "UserId", "CustomDataKey" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Theory]
    [MemberData(nameof(ExpectedIndexes))]
    public void Configure_CreatesExpectedIndex(string[] propertyNames)
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<UserData>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, propertyNames);

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_ItemForeignKey_UsesUserDataNavigation()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<UserData>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(BaseItemEntity));

        Assert.Equal("Item", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("UserData", foreignKey.PrincipalToDependent?.Name);
    }
}
