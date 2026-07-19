using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class PreferenceConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueFilteredUserIdKindIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<Preference>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "UserId", "Kind");

        Assert.True(index.IsUnique);
        Assert.Equal("[UserId] IS NOT NULL", index.GetFilter());
    }
}
