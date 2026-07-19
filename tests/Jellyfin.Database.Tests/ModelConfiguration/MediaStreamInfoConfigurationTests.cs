using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class MediaStreamInfoConfigurationTests
{
    [Fact]
    public void Configure_SetsCompositePrimaryKey()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<MediaStreamInfo>();

        Assert.Equal(new[] { "ItemId", "StreamIndex" }, entityType.FindPrimaryKey()!.Properties.Select(p => p.Name));
    }
}
