using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class HomeSectionTests
    {
        [Fact]
        public void Properties_HaveExpectedDefaults()
        {
            var section = new HomeSection();

            Assert.Equal(0, section.Id);
            Assert.Equal(0, section.DisplayPreferencesId);
            Assert.Equal(0, section.Order);
            Assert.Equal(HomeSectionType.None, section.Type);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var section = new HomeSection
            {
                DisplayPreferencesId = 5,
                Order = 2,
                Type = HomeSectionType.NextUp
            };

            Assert.Equal(5, section.DisplayPreferencesId);
            Assert.Equal(2, section.Order);
            Assert.Equal(HomeSectionType.NextUp, section.Type);
        }
    }
}
