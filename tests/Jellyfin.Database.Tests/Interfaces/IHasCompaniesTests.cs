using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Interfaces;
using Xunit;

namespace Jellyfin.Database.Tests.Interfaces;

public class IHasCompaniesTests
{
    [Fact]
    public void MovieMetadata_ImplementsIHasCompanies()
    {
        var metadata = new MovieMetadata("Title", "eng");

        Assert.IsAssignableFrom<IHasCompanies>(metadata);
    }

    [Fact]
    public void MovieMetadata_Companies_AliasesStudios()
    {
        var metadata = new MovieMetadata("Title", "eng");
        var company = new Company();

        ((IHasCompanies)metadata).Companies.Add(company);

        Assert.Single(metadata.Studios, company);
    }

    [Fact]
    public void Company_Companies_AliasesChildCompanies()
    {
        var company = new Company();
        var child = new Company();

        ((IHasCompanies)company).Companies.Add(child);

        Assert.Single(company.ChildCompanies, child);
    }

    [Fact]
    public void Companies_StartsEmpty()
    {
        IHasCompanies company = new Company();

        Assert.Empty(company.Companies);
    }
}
