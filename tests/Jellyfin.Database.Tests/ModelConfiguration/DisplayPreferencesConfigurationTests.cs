using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class DisplayPreferencesConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueUserItemClientIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<DisplayPreferences>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "UserId", "ItemId", "Client");

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Configure_HomeSections_CascadeOnDelete()
    {
        var homeSectionType = ModelConfigurationTestHelper.GetEntityType<HomeSection>();

        var foreignKey = homeSectionType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(DisplayPreferences));

        Assert.Equal("HomeSections", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }
}
