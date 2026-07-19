using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class AttachmentStreamInfoConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<AttachmentStreamInfo>();

        Assert.Equal(new[] { "ItemId", "Index" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }
}
