using System.IO;
using CommandLine;
using Xunit;
using static MediaBrowser.Controller.Extensions.ConfigurationExtensions;

namespace Jellyfin.Server.Tests;

public class StartupOptionsTests
{
    [Fact]
    public void ParseArguments_LongOptions_MapToProperties()
    {
        var args = new[]
        {
            "--datadir", "/data",
            "--webdir", "/web",
            "--cachedir", "/cache",
            "--configdir", "/config",
            "--logdir", "/log",
            "--ffmpeg", "/usr/bin/ffmpeg",
            "--package-name", "synology",
            "--published-server-url", "https://jellyfin.example.com",
            "--nowebclient",
            "--service",
            "--nonetchange"
        };

        var options = Parse(args);

        Assert.Equal("/data", options.DataDir);
        Assert.Equal("/web", options.WebDir);
        Assert.Equal("/cache", options.CacheDir);
        Assert.Equal("/config", options.ConfigDir);
        Assert.Equal("/log", options.LogDir);
        Assert.Equal("/usr/bin/ffmpeg", options.FFmpegPath);
        Assert.Equal("synology", options.PackageName);
        Assert.Equal("https://jellyfin.example.com", options.PublishedServerUrl);
        Assert.True(options.NoWebClient);
        Assert.True(options.IsService);
        Assert.True(options.NoDetectNetworkChange);
    }

    [Fact]
    public void ParseArguments_ShortOptions_MapToProperties()
    {
        var args = new[] { "-d", "/data", "-w", "/web", "-C", "/cache", "-c", "/config", "-l", "/log" };

        var options = Parse(args);

        Assert.Equal("/data", options.DataDir);
        Assert.Equal("/web", options.WebDir);
        Assert.Equal("/cache", options.CacheDir);
        Assert.Equal("/config", options.ConfigDir);
        Assert.Equal("/log", options.LogDir);
    }

    [Fact]
    public void ConvertToConfig_DefaultOptions_ReturnsEmptyDictionary()
    {
        var options = new StartupOptions();

        Assert.Empty(options.ConvertToConfig());
    }

    [Fact]
    public void ConvertToConfig_AllRelevantOptionsSet_MapsToConfigurationKeys()
    {
        var options = new StartupOptions
        {
            NoWebClient = true,
            PublishedServerUrl = "https://jellyfin.example.com",
            FFmpegPath = "/usr/bin/ffmpeg",
            NoDetectNetworkChange = true
        };

        var config = options.ConvertToConfig();

        Assert.Equal(4, config.Count);
        Assert.Equal(bool.FalseString, config[HostWebClientKey]);
        Assert.Equal("https://jellyfin.example.com", config[AddressOverrideKey]);
        Assert.Equal("/usr/bin/ffmpeg", config[FfmpegPathKey]);
        Assert.Equal(bool.FalseString, config[DetectNetworkChangeKey]);
    }

    private static StartupOptions Parse(params string[] args)
    {
        using var parser = new Parser(settings => settings.HelpWriter = TextWriter.Null);
        var result = parser.ParseArguments<StartupOptions>(args);

        Assert.Empty(result.Errors);
        return result.Value;
    }
}
