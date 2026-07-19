using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite;

public class DoNotUseReturningClauseConventionTests
{
    private const string UseSqlReturningClauseAnnotation = "Sqlite:UseSqlReturningClause";

    private static IModelFinalizingConvention CreateConvention()
    {
        var conventionType = typeof(Jellyfin.Database.Providers.Sqlite.SqliteDatabaseProvider).Assembly
            .GetType("Jellyfin.Database.Providers.Sqlite.DoNotUseReturningClauseConvention");
        Assert.NotNull(conventionType);

        return Assert.IsAssignableFrom<IModelFinalizingConvention>(Activator.CreateInstance(conventionType));
    }

    [Fact]
    public void ProcessModelFinalizing_DisablesReturningClauseForAllEntityTypes()
    {
        var conventionSet = SqliteConventionSetBuilder.Build();
        conventionSet.ModelFinalizingConventions.Add(CreateConvention());

        var modelBuilder = new ModelBuilder(conventionSet);
        modelBuilder.Entity<FirstEntity>();
        modelBuilder.Entity<SecondEntity>();

        var model = modelBuilder.FinalizeModel();

        foreach (var entityType in model.GetEntityTypes())
        {
            var annotation = entityType.FindAnnotation(UseSqlReturningClauseAnnotation);
            Assert.NotNull(annotation);
            Assert.Equal(false, annotation.Value);
        }
    }

    [Fact]
    public void WithoutConvention_ReturningClauseAnnotationIsAbsent()
    {
        var modelBuilder = new ModelBuilder(SqliteConventionSetBuilder.Build());
        modelBuilder.Entity<FirstEntity>();

        var model = modelBuilder.FinalizeModel();

        var entityType = model.FindEntityType(typeof(FirstEntity));
        Assert.NotNull(entityType);
        Assert.Null(entityType.FindAnnotation(UseSqlReturningClauseAnnotation));
    }

    private sealed class FirstEntity
    {
        public int Id { get; set; }
    }

    private sealed class SecondEntity
    {
        public int Id { get; set; }
    }
}
