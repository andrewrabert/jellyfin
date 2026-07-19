using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class PermissionConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueFilteredUserIdKindIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<Permission>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "UserId", "Kind");

        Assert.True(index.IsUnique);
        Assert.Equal("[UserId] IS NOT NULL", index.GetFilter());
    }
}
