using Jellyfin.Database.Implementations.Entities.Security;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class ApiKeyConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueAccessTokenIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<ApiKey>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "AccessToken");

        Assert.True(index.IsUnique);
    }
}
