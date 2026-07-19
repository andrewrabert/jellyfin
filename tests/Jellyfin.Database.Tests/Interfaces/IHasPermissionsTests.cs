using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Interfaces;

public class IHasPermissionsTests
{
    private static User CreateUser()
        => new User("username", "authProvider", "resetProvider");

    [Fact]
    public void User_ImplementsIHasPermissions()
    {
        Assert.IsAssignableFrom<IHasPermissions>(CreateUser());
    }

    [Fact]
    public void Permissions_StartsEmpty()
    {
        IHasPermissions user = CreateUser();

        Assert.Empty(user.Permissions);
    }

    [Fact]
    public void Permissions_CanAddItems()
    {
        IHasPermissions user = CreateUser();

        user.Permissions.Add(new Permission(PermissionKind.IsAdministrator, true));
        user.Permissions.Add(new Permission(PermissionKind.EnableMediaPlayback, false));

        Assert.Equal(2, user.Permissions.Count);
        Assert.Contains(user.Permissions, p => p.Kind == PermissionKind.IsAdministrator && p.Value);
        Assert.Contains(user.Permissions, p => p.Kind == PermissionKind.EnableMediaPlayback && !p.Value);
    }
}
