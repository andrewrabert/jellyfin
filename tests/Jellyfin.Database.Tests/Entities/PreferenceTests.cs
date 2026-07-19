using System;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class PreferenceTests
    {
        [Fact]
        public void Constructor_SetsKindAndValue()
        {
            var preference = new Preference(PreferenceKind.BlockedTags, "tag1|tag2");

            Assert.Equal(PreferenceKind.BlockedTags, preference.Kind);
            Assert.Equal("tag1|tag2", preference.Value);
        }

        [Fact]
        public void Constructor_NullValue_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Preference(PreferenceKind.BlockedTags, null!));
        }

        [Fact]
        public void Constructor_SetsDefaults()
        {
            var preference = new Preference(PreferenceKind.BlockedTags, string.Empty);

            Assert.Equal(0, preference.Id);
            Assert.Null(preference.UserId);
            Assert.Equal(0u, preference.RowVersion);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var userId = Guid.NewGuid();
            var preference = new Preference(PreferenceKind.BlockedTags, "old")
            {
                UserId = userId,
                Value = "new"
            };

            Assert.Equal(userId, preference.UserId);
            Assert.Equal("new", preference.Value);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var preference = new Preference(PreferenceKind.BlockedTags, "value");

            preference.OnSavingChanges();
            Assert.Equal(1u, preference.RowVersion);

            preference.OnSavingChanges();
            Assert.Equal(2u, preference.RowVersion);
        }
    }
}
