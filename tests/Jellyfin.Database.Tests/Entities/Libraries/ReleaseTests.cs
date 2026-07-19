using System;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class ReleaseTests
    {
        [Fact]
        public void Ctor_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Release(null!));
        }

        [Fact]
        public void Ctor_EmptyName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Release(string.Empty));
        }

        [Fact]
        public void Ctor_SetsName()
        {
            var release = new Release("Director's Cut");

            Assert.Equal("Director's Cut", release.Name);
        }

        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var release = new Release("Director's Cut");

            Assert.NotNull(release.MediaFiles);
            Assert.Empty(release.MediaFiles);
            Assert.NotNull(release.Chapters);
            Assert.Empty(release.Chapters);
        }

        [Fact]
        public void Name_RoundTrips()
        {
            var release = new Release("Director's Cut")
            {
                Name = "Theatrical"
            };

            Assert.Equal("Theatrical", release.Name);
        }

        [Fact]
        public void Release_ImplementsIHasConcurrencyToken()
        {
            Assert.IsAssignableFrom<IHasConcurrencyToken>(new Release("Standard"));
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Release("Standard");

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
