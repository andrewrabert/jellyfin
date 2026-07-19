using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Emby.Server.Implementations;
using Jellyfin.Server.Migrations.PreStartupRoutines;
using MediaBrowser.Providers.Plugins.MusicBrainz.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Server.Tests.Migrations.PreStartupRoutines;

public sealed class MigrateMusicBrainzTimeoutTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();
    private readonly ServerApplicationPaths _appPaths;
    private readonly string _pluginConfigPath;

    public MigrateMusicBrainzTimeoutTests()
    {
        _appPaths = new ServerApplicationPaths(
            _tempDirectory.FullName,
            Path.Combine(_tempDirectory.FullName, "log"),
            Path.Combine(_tempDirectory.FullName, "config"),
            Path.Combine(_tempDirectory.FullName, "cache"),
            Path.Combine(_tempDirectory.FullName, "web"));
        Directory.CreateDirectory(_appPaths.PluginConfigurationsPath);
        _pluginConfigPath = Path.Combine(_appPaths.PluginConfigurationsPath, "Jellyfin.Plugin.MusicBrainz.xml");
    }

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    [Fact]
    public void Perform_NoConfigFile_DoesNotCreateOne()
    {
        Perform();

        Assert.False(File.Exists(_pluginConfigPath));
    }

    [Fact]
    public void Perform_OldConfig_ConvertsRateLimitFromMillisecondsToSeconds()
    {
        WriteOldConfig(server: "https://musicbrainz.example.com", rateLimit: 2000, replaceArtistName: true);

        Perform();
        var newConfig = ReadNewConfig();

        Assert.Equal("https://musicbrainz.example.com", newConfig.Server);
        Assert.Equal(2.0, newConfig.RateLimit);
        Assert.True(newConfig.ReplaceArtistName);
    }

    [Fact]
    public void Perform_OldConfigWithLowRateLimit_ClampsToOneSecond()
    {
        WriteOldConfig(server: "https://musicbrainz.org", rateLimit: 200, replaceArtistName: false);

        Perform();
        var newConfig = ReadNewConfig();

        Assert.Equal(1.0, newConfig.RateLimit);
        Assert.False(newConfig.ReplaceArtistName);
    }

    private void Perform()
        => new MigrateMusicBrainzTimeout(_appPaths, NullLoggerFactory.Instance).Perform();

    private void WriteOldConfig(string server, long rateLimit, bool replaceArtistName)
    {
        File.WriteAllText(
            _pluginConfigPath,
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <PluginConfiguration>
              <Server>{server}</Server>
              <RateLimit>{rateLimit}</RateLimit>
              <ReplaceArtistName>{(replaceArtistName ? "true" : "false")}</ReplaceArtistName>
            </PluginConfiguration>
            """);
    }

    private PluginConfiguration ReadNewConfig()
    {
        var serializer = new XmlSerializer(typeof(PluginConfiguration), new XmlRootAttribute("PluginConfiguration"));
        using var xmlReader = XmlReader.Create(_pluginConfigPath);
        return Assert.IsType<PluginConfiguration>(serializer.Deserialize(xmlReader));
    }
}
