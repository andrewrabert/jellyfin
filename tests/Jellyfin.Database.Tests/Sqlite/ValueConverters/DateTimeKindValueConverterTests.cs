using System;
using Jellyfin.Database.Providers.Sqlite.ValueConverters;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite.ValueConverters;

public class DateTimeKindValueConverterTests
{
    [Fact]
    public void ConvertToProvider_UtcValue_IsUnchanged()
    {
        var converter = new DateTimeKindValueConverter(DateTimeKind.Utc);
        var value = new DateTime(2025, 5, 1, 12, 30, 0, DateTimeKind.Utc);

        var result = Assert.IsType<DateTime>(converter.ConvertToProvider(value));

        Assert.Equal(value, result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void ConvertToProvider_LocalValue_ConvertsToUniversalTime()
    {
        var converter = new DateTimeKindValueConverter(DateTimeKind.Utc);
        var value = new DateTime(2025, 5, 1, 12, 30, 0, DateTimeKind.Local);

        var result = Assert.IsType<DateTime>(converter.ConvertToProvider(value));

        Assert.Equal(value.ToUniversalTime(), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Theory]
    [InlineData(DateTimeKind.Utc)]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void ConvertFromProvider_SpecifiesConfiguredKind(DateTimeKind kind)
    {
        var converter = new DateTimeKindValueConverter(kind);
        var value = new DateTime(2025, 5, 1, 12, 30, 0, DateTimeKind.Unspecified);

        var result = Assert.IsType<DateTime>(converter.ConvertFromProvider(value));

        Assert.Equal(kind, result.Kind);
        Assert.Equal(value.Ticks, result.Ticks);
    }

    [Fact]
    public void RoundTrip_UtcValue_IsPreserved()
    {
        var converter = new DateTimeKindValueConverter(DateTimeKind.Utc);
        var value = new DateTime(2025, 5, 1, 12, 30, 0, DateTimeKind.Utc);

        var stored = Assert.IsType<DateTime>(converter.ConvertToProvider(value));
        var restored = Assert.IsType<DateTime>(converter.ConvertFromProvider(stored));

        Assert.Equal(value, restored);
        Assert.Equal(DateTimeKind.Utc, restored.Kind);
    }
}
