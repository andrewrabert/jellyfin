using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class TrickplayInfoConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<TrickplayInfo>();

        Assert.Equal(new[] { "ItemId", "Width" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }
}
