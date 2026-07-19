using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class GroupTests
    {
        [Fact]
        public void Constructor_SetsName()
        {
            var group = new Group("admins");

            Assert.Equal("admins", group.Name);
        }

        [Fact]
        public void Constructor_GeneratesNonEmptyId()
        {
            var group = new Group("admins");

            Assert.NotEqual(Guid.Empty, group.Id);
        }

        [Fact]
        public void Constructor_GeneratesUniqueIds()
        {
            Assert.NotEqual(new Group("a").Id, new Group("b").Id);
        }

        [Fact]
        public void Constructor_InitializesEmptyCollections()
        {
            var group = new Group("admins");

            Assert.NotNull(group.Permissions);
            Assert.Empty(group.Permissions);
            Assert.NotNull(group.Preferences);
            Assert.Empty(group.Preferences);
        }

        [Fact]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Group(null!));
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Group(string.Empty));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var group = new Group("admins");
            Assert.Equal(0u, group.RowVersion);

            group.OnSavingChanges();
            Assert.Equal(1u, group.RowVersion);

            group.OnSavingChanges();
            Assert.Equal(2u, group.RowVersion);
        }
    }
}
