using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class LinkedChildConfigurationTests
{
    public static TheoryData<string[]> ExpectedIndexes => new()
    {
        new[] { "ParentId", "SortOrder" },
        new[] { "ParentId", "ChildType" },
        new[] { "ChildId", "ChildType" }
    };

    [Fact]
    public void Configure_MapsToLinkedChildrenTable()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<LinkedChildEntity>();

        Assert.Equal("LinkedChildren", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<LinkedChildEntity>();

        Assert.Equal(new[] { "ParentId", "ChildId" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Theory]
    [MemberData(nameof(ExpectedIndexes))]
    public void Configure_CreatesExpectedIndex(string[] propertyNames)
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<LinkedChildEntity>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, propertyNames);

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_ParentForeignKey_DoesNotCascadeOnDelete()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<LinkedChildEntity>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ParentId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Parent", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("LinkedChildEntities", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.NoAction, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_ChildForeignKey_DoesNotCascadeOnDelete()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<LinkedChildEntity>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ChildId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Child", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("LinkedChildOfEntities", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.NoAction, foreignKey.DeleteBehavior);
    }
}
