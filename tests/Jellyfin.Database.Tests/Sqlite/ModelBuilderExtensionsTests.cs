using System;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Database.Providers.Sqlite.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite;

public class ModelBuilderExtensionsTests
{
    private static ModelBuilder CreateModelBuilder()
    {
        var modelBuilder = new ModelBuilder(SqliteConventionSetBuilder.Build());
        modelBuilder.Entity<TestEntity>();
        return modelBuilder;
    }

    [Fact]
    public void UseValueConverterForType_AppliesConverterToMatchingProperties()
    {
        var modelBuilder = CreateModelBuilder();
        var converter = new DateTimeKindValueConverter(DateTimeKind.Utc);

        var result = modelBuilder.UseValueConverterForType<DateTime>(converter);

        Assert.Same(modelBuilder, result);
        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        Assert.NotNull(entityType);
        Assert.Same(converter, entityType.FindProperty(nameof(TestEntity.Created))!.GetValueConverter());
    }

    [Fact]
    public void UseValueConverterForType_DoesNotTouchOtherPropertyTypes()
    {
        var modelBuilder = CreateModelBuilder();

        modelBuilder.UseValueConverterForType<DateTime>(new DateTimeKindValueConverter(DateTimeKind.Utc));

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        Assert.NotNull(entityType);
        Assert.Null(entityType.FindProperty(nameof(TestEntity.Name))!.GetValueConverter());
        Assert.Null(entityType.FindProperty(nameof(TestEntity.Modified))!.GetValueConverter());
    }

    [Fact]
    public void SetDefaultDateTimeKind_AppliesConverterToNullableAndNonNullableDateTimes()
    {
        var modelBuilder = CreateModelBuilder();

        modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
        Assert.NotNull(entityType);
        Assert.IsType<DateTimeKindValueConverter>(entityType.FindProperty(nameof(TestEntity.Created))!.GetValueConverter());
        Assert.IsType<DateTimeKindValueConverter>(entityType.FindProperty(nameof(TestEntity.Modified))!.GetValueConverter());
        Assert.Null(entityType.FindProperty(nameof(TestEntity.Name))!.GetValueConverter());
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
