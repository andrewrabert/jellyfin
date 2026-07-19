using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Entities.Libraries
{
    public class CompanyTests
    {
        [Fact]
        public void Ctor_InitializesEmptyCollections()
        {
            var company = new Company();

            Assert.NotNull(company.CompanyMetadata);
            Assert.Empty(company.CompanyMetadata);
            Assert.NotNull(company.ChildCompanies);
            Assert.Empty(company.ChildCompanies);
        }

        [Fact]
        public void Companies_ReturnsChildCompanies()
        {
            var company = new Company();

            Assert.Same(company.ChildCompanies, company.Companies);
        }

        [Fact]
        public void Company_ImplementsExpectedInterfaces()
        {
            var company = new Company();

            Assert.IsAssignableFrom<IHasCompanies>(company);
            Assert.IsAssignableFrom<IHasConcurrencyToken>(company);
        }

        [Fact]
        public void OnSavingChanges_IncrementsRowVersion()
        {
            var entity = new Company();

            Assert.Equal(0u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(1u, entity.RowVersion);

            entity.OnSavingChanges();
            Assert.Equal(2u, entity.RowVersion);
        }
    }
}
