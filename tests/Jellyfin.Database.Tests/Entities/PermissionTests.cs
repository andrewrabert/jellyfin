using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class PermissionTests
    {
        [Theory]
        [InlineData(PermissionKind.IsAdministrator, true)]
        [InlineData(PermissionKind.EnableMediaPlayback, false)]
        public void Constructor_SetsKindAndValue(PermissionKind kind, bool value)
        {
            var permission = new Permission(kind, value);

            Assert.Equal(kind, permission.Kind);
            Assert.Equal(value, permission.Value);
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var permission = new Permission(PermissionKind.IsAdministrator, true);

            Assert.Equal(0, permission.Id);
            Assert.Null(permission.UserId);
            Assert.Equal(0u, permission.RowVersion);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var userId = Guid.NewGuid();
            var permission = new Permission(PermissionKind.IsAdministrator, false)
            {
                UserId = userId,
                Value = true
            };

            Assert.Equal(userId, permission.UserId);
            Assert.True(permission.Value);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var permission = new Permission(PermissionKind.IsAdministrator, true);

            permission.OnSavingChanges();
            Assert.Equal(1u, permission.RowVersion);

            permission.OnSavingChanges();
            Assert.Equal(2u, permission.RowVersion);
        }
    }
}
