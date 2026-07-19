using System;
using System.Linq;
using System.Reflection;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Providers.Sqlite;
using Xunit;

namespace Jellyfin.Database.Tests;

public class JellyfinDatabaseProviderKeyAttributeTests
{
    [Fact]
    public void Constructor_StoresProviderKey()
    {
        var attribute = new JellyfinDatabaseProviderKeyAttribute("My-Provider");

        Assert.Equal("My-Provider", attribute.DatabaseProviderKey);
    }

    [Fact]
    public void AttributeUsage_TargetsClassesAndAllowsMultiple()
    {
        var usage = typeof(JellyfinDatabaseProviderKeyAttribute).GetCustomAttribute<AttributeUsageAttribute>();

        Assert.NotNull(usage);
        Assert.Equal(AttributeTargets.Class, usage.ValidOn);
        Assert.True(usage.AllowMultiple);
        Assert.True(usage.Inherited);
    }

    [Fact]
    public void IsSealed()
    {
        Assert.True(typeof(JellyfinDatabaseProviderKeyAttribute).IsSealed);
    }

    [Fact]
    public void SqliteProvider_IsAnnotatedWithSqliteKey()
    {
        var keys = typeof(SqliteDatabaseProvider)
            .GetCustomAttributes<JellyfinDatabaseProviderKeyAttribute>()
            .Select(a => a.DatabaseProviderKey);

        Assert.Contains("Jellyfin-SQLite", keys);
    }
}
