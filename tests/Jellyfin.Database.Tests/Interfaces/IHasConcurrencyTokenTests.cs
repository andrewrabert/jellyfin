using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Interfaces;

public class IHasConcurrencyTokenTests
{
    [Fact]
    public void RowVersion_StartsAtZero()
    {
        IHasConcurrencyToken library = new Library("Name", "/path");

        Assert.Equal(0u, library.RowVersion);
    }

    [Fact]
    public void OnSavingChanges_IncrementsRowVersion()
    {
        IHasConcurrencyToken library = new Library("Name", "/path");

        library.OnSavingChanges();

        Assert.Equal(1u, library.RowVersion);
    }

    [Fact]
    public void OnSavingChanges_IncrementsRowVersionEachCall()
    {
        IHasConcurrencyToken artwork = new Artwork("/some/path.jpg", ArtKind.Poster);

        artwork.OnSavingChanges();
        artwork.OnSavingChanges();
        artwork.OnSavingChanges();

        Assert.Equal(3u, artwork.RowVersion);
    }

    [Fact]
    public void Permission_ImplementsIHasConcurrencyToken()
    {
        IHasConcurrencyToken permission = new Permission(PermissionKind.IsAdministrator, true);

        permission.OnSavingChanges();

        Assert.Equal(1u, permission.RowVersion);
    }
}
