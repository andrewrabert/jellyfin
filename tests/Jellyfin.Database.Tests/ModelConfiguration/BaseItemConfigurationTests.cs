using System;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class BaseItemConfigurationTests
{
    public static TheoryData<string[]> ExpectedIndexes => new()
    {
        new[] { "Path" },
        new[] { "ParentId" },
        new[] { "OwnerId" },
        new[] { "Name" },
        new[] { "ExtraType", "OwnerId" },
        new[] { "PresentationUniqueKey" },
        new[] { "TopParentId", "Id" },
        new[] { "Type", "SeriesPresentationUniqueKey", "PresentationUniqueKey", "SortName" },
        new[] { "Type", "SeriesPresentationUniqueKey", "IsFolder", "IsVirtualItem" },
        new[] { "Type", "TopParentId", "StartDate" },
        new[] { "Type", "TopParentId", "Id" },
        new[] { "Type", "TopParentId", "PresentationUniqueKey" },
        new[] { "Type", "TopParentId", "IsVirtualItem", "PresentationUniqueKey", "DateCreated" },
        new[] { "IsFolder", "TopParentId", "IsVirtualItem", "PresentationUniqueKey", "DateCreated" },
        new[] { "TopParentId", "Type", "IsVirtualItem", "DateCreated" },
        new[] { "TopParentId", "IsFolder", "IsVirtualItem", "DateCreated" },
        new[] { "TopParentId", "MediaType", "IsVirtualItem", "DateCreated" },
        new[] { "MediaType", "TopParentId", "IsVirtualItem", "PresentationUniqueKey" },
        new[] { "Type", "TopParentId", "SortName" },
        new[] { "Type", "SeriesPresentationUniqueKey", "ParentIndexNumber", "IndexNumber" },
        new[] { "Type", "CleanName" },
        new[] { "SeriesName" },
        new[] { "SeasonId" },
        new[] { "SeriesId" }
    };

    [Fact]
    public void Configure_UsesIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        Assert.Equal(new[] { "Id" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }

    [Theory]
    [MemberData(nameof(ExpectedIndexes))]
    public void Configure_CreatesExpectedIndex(string[] propertyNames)
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, propertyNames);

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_ItemCountsIndex_IsFiltered()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "TopParentId", "Type", "IsVirtualItem");

        Assert.Equal("\"PrimaryVersionId\" IS NULL AND (\"OwnerId\" IS NULL OR \"ExtraType\" IS NOT NULL)", index.GetFilter());
    }

    [Fact]
    public void Configure_DirectChildrenForeignKey_CascadesOnDelete()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "ParentId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("DirectParent", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("DirectChildren", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_ExtrasForeignKey_DoesNotCascadeOnDelete()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        var foreignKey = entityType.GetForeignKeys()
            .Single(fk => fk.Properties.Select(p => p.Name).SequenceEqual(new[] { "OwnerId" }));

        Assert.Equal(typeof(BaseItemEntity), foreignKey.PrincipalEntityType.ClrType);
        Assert.Equal("Owner", foreignKey.DependentToPrincipal?.Name);
        Assert.Equal("Extras", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.NoAction, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_SeedsPlaceholderItem()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<BaseItemEntity>();

        var seed = Assert.Single(entityType.GetSeedData());

        Assert.Equal(Guid.Parse("00000000-0000-0000-0000-000000000001"), seed["Id"]);
        Assert.Equal("PLACEHOLDER", seed["Type"]);
    }
}
