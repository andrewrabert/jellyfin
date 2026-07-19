using System;
using System.Linq;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jellyfin.Database.Tests.ModelConfiguration;

internal static class ModelConfigurationTestHelper
{
    private static readonly Lazy<IModel> _model = new(BuildModel);

    public static IEntityType GetEntityType<TEntity>()
        => _model.Value.FindEntityType(typeof(TEntity))
           ?? throw new InvalidOperationException($"Entity type {typeof(TEntity)} is not part of the model.");

    public static IIndex GetIndex(IEntityType entityType, params string[] propertyNames)
        => entityType.GetIndexes().Single(i => i.Properties.Select(p => p.Name).SequenceEqual(propertyNames));

    private static IModel BuildModel()
    {
        var options = new DbContextOptionsBuilder<JellyfinDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new JellyfinDbContext(
            options,
            NullLogger<JellyfinDbContext>.Instance,
            new SqliteDatabaseProvider(null!, NullLogger<SqliteDatabaseProvider>.Instance),
            new NoLockBehavior(NullLogger<NoLockBehavior>.Instance));

        // The design-time model retains configuration (e.g. seed data) that the
        // read-optimized runtime model does not expose.
        return context.GetService<IDesignTimeModel>().Model;
    }
}
