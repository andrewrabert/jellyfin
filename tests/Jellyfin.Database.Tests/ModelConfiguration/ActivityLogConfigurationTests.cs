using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class ActivityLogConfigurationTests
{
    [Fact]
    public void Configure_CreatesDateCreatedIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ActivityLog>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "DateCreated");

        Assert.False(index.IsUnique);
    }

    [Fact]
    public void Configure_UsesIdAsPrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ActivityLog>();

        Assert.Equal(new[] { "Id" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }
}
