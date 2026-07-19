using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jellyfin.Database.Tests.ModelConfiguration;

public class UserConfigurationTests
{
    [Fact]
    public void Configure_CreatesUniqueUsernameIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<User>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "Username");

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Configure_CreatesUniqueNormalizedUsernameIndex()
    {
        var entityType = ModelConfigurationTestHelper.GetEntityType<User>();

        var index = ModelConfigurationTestHelper.GetIndex(entityType, "NormalizedUsername");

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void Configure_ProfileImage_CascadesOnDelete()
    {
        var imageInfoType = ModelConfigurationTestHelper.GetEntityType<ImageInfo>();

        var foreignKey = imageInfoType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.True(foreignKey.IsUnique);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_Permissions_CascadeOnDelete()
    {
        var permissionType = ModelConfigurationTestHelper.GetEntityType<Permission>();

        var foreignKey = permissionType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.Equal(new[] { "UserId" }, foreignKey.Properties.Select(p => p.Name));
        Assert.Equal("Permissions", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_Preferences_CascadeOnDelete()
    {
        var preferenceType = ModelConfigurationTestHelper.GetEntityType<Preference>();

        var foreignKey = preferenceType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.Equal(new[] { "UserId" }, foreignKey.Properties.Select(p => p.Name));
        Assert.Equal("Preferences", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_AccessSchedules_CascadeOnDelete()
    {
        var accessScheduleType = ModelConfigurationTestHelper.GetEntityType<AccessSchedule>();

        var foreignKey = accessScheduleType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.Equal("AccessSchedules", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_DisplayPreferences_CascadeOnDelete()
    {
        var displayPreferencesType = ModelConfigurationTestHelper.GetEntityType<DisplayPreferences>();

        var foreignKey = displayPreferencesType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.Equal("DisplayPreferences", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }

    [Fact]
    public void Configure_ItemDisplayPreferences_CascadeOnDelete()
    {
        var itemDisplayPreferencesType = ModelConfigurationTestHelper.GetEntityType<ItemDisplayPreferences>();

        var foreignKey = itemDisplayPreferencesType.GetForeignKeys()
            .Single(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        Assert.Equal("ItemDisplayPreferences", foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }
}
