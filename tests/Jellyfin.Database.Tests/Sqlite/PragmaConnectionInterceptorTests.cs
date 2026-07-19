using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Providers.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite;

public class PragmaConnectionInterceptorTests
{
    private static PragmaConnectionInterceptor CreateInterceptor(IDictionary<string, string>? customPragma = null)
        => new(
            NullLogger.Instance,
            cacheSize: -2500,
            lockingMode: "NORMAL",
            journalSizeLimit: 65536,
            tempStoreMode: 2,
            syncMode: 1,
            customPragma ?? new Dictionary<string, string>());

    private static long QueryPragma(SqliteConnection connection, string pragma)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {pragma};";
        return Convert.ToInt64(command.ExecuteScalar(), System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string QueryPragmaText(SqliteConnection connection, string pragma)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {pragma};";
        return (string)command.ExecuteScalar()!;
    }

    [Fact]
    public void ConnectionOpened_AppliesConfiguredPragmas()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        CreateInterceptor().ConnectionOpened(connection, null!);

        Assert.Equal(-2500, QueryPragma(connection, "cache_size"));
        Assert.Equal("normal", QueryPragmaText(connection, "locking_mode"));
        Assert.Equal(65536, QueryPragma(connection, "journal_size_limit"));
        Assert.Equal(1, QueryPragma(connection, "synchronous"));
        Assert.Equal(2, QueryPragma(connection, "temp_store"));
    }

    [Fact]
    public void ConnectionOpened_AppliesCustomPragmas()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        CreateInterceptor(new Dictionary<string, string> { ["user_version"] = "42" })
            .ConnectionOpened(connection, null!);

        Assert.Equal(42, QueryPragma(connection, "user_version"));
    }

    [Fact]
    public async Task ConnectionOpenedAsync_AppliesConfiguredPragmas()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await using (connection.ConfigureAwait(true))
        {
            await connection.OpenAsync(TestContext.Current.CancellationToken);

            await CreateInterceptor().ConnectionOpenedAsync(connection, null!, TestContext.Current.CancellationToken);

            Assert.Equal(-2500, QueryPragma(connection, "cache_size"));
            Assert.Equal(1, QueryPragma(connection, "synchronous"));
            Assert.Equal(2, QueryPragma(connection, "temp_store"));
        }
    }
}
