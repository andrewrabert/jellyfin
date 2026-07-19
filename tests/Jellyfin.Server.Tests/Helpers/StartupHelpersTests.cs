using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jellyfin.Server.Helpers;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Tests.Helpers;

public sealed class StartupHelpersTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    [Fact]
    public void GetUnixSocketPath_PathConfigured_ReturnsConfiguredPath()
    {
        var startupConfig = BuildConfiguration(new Dictionary<string, string?>
        {
            [MediaBrowser.Controller.Extensions.ConfigurationExtensions.UnixSocketPathKey] = "/run/jellyfin/custom.sock"
        });

        var socketPath = StartupHelpers.GetUnixSocketPath(startupConfig, GetMockAppPaths());

        Assert.Equal("/run/jellyfin/custom.sock", socketPath);
    }

    [Fact]
    public void GetUnixSocketPath_NoConfiguredPath_FallsBackToXdgRuntimeDir()
    {
        var originalValue = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        try
        {
            Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", "/run/user/1000");

            var socketPath = StartupHelpers.GetUnixSocketPath(BuildConfiguration(new Dictionary<string, string?>()), GetMockAppPaths());

            Assert.Equal(Path.Join("/run/user/1000", "jellyfin.sock"), socketPath);
        }
        finally
        {
            Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", originalValue);
        }
    }

    [Fact]
    public void GetUnixSocketPath_NoConfiguredPathNoXdgRuntimeDir_FallsBackToConfigDir()
    {
        var originalValue = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
        try
        {
            Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", null);

            var socketPath = StartupHelpers.GetUnixSocketPath(BuildConfiguration(new Dictionary<string, string?>()), GetMockAppPaths());

            Assert.Equal(Path.Join(_tempDirectory.FullName, "jellyfin.sock"), socketPath);
        }
        finally
        {
            Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", originalValue);
        }
    }

    [Fact]
    public void SetUnixSocketPermissions_PermissionsConfigured_SetsFileMode()
    {
        if (OperatingSystem.IsWindows())
        {
            Assert.Skip("Unix file modes are not supported on Windows.");
        }
        else
        {
            var socketPath = Path.Combine(_tempDirectory.FullName, "jellyfin.sock");
            File.Create(socketPath).Dispose();
            var startupConfig = BuildConfiguration(new Dictionary<string, string?>
            {
                [MediaBrowser.Controller.Extensions.ConfigurationExtensions.UnixSocketPermissionsKey] = "660"
            });

            StartupHelpers.SetUnixSocketPermissions(startupConfig, socketPath, NullLogger.Instance);

            Assert.Equal(
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.GroupRead | UnixFileMode.GroupWrite,
                File.GetUnixFileMode(socketPath));
        }
    }

    [Fact]
    public void SetUnixSocketPermissions_NoPermissionsConfigured_LeavesFileModeUnchanged()
    {
        if (OperatingSystem.IsWindows())
        {
            Assert.Skip("Unix file modes are not supported on Windows.");
        }
        else
        {
            var socketPath = Path.Combine(_tempDirectory.FullName, "jellyfin.sock");
            File.Create(socketPath).Dispose();
            var originalMode = File.GetUnixFileMode(socketPath);

            StartupHelpers.SetUnixSocketPermissions(BuildConfiguration(new Dictionary<string, string?>()), socketPath, NullLogger.Instance);

            Assert.Equal(originalMode, File.GetUnixFileMode(socketPath));
        }
    }

    [Fact]
    public async Task InitLoggingConfigFile_FileMissing_CreatesDefaultConfigFromResource()
    {
        var configPath = Path.Combine(_tempDirectory.FullName, Program.LoggingConfigFileDefault);

        await StartupHelpers.InitLoggingConfigFile(GetMockAppPaths());

        Assert.True(File.Exists(configPath));
        Assert.Contains("Serilog", await File.ReadAllTextAsync(configPath, TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    [Fact]
    public async Task InitLoggingConfigFile_FileExists_LeavesFileUntouched()
    {
        var configPath = Path.Combine(_tempDirectory.FullName, Program.LoggingConfigFileDefault);
        await File.WriteAllTextAsync(configPath, "{}", TestContext.Current.CancellationToken);

        await StartupHelpers.InitLoggingConfigFile(GetMockAppPaths());

        Assert.Equal("{}", await File.ReadAllTextAsync(configPath, TestContext.Current.CancellationToken));
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private IApplicationPaths GetMockAppPaths()
    {
        var appPaths = new Mock<IApplicationPaths>();
        appPaths.Setup(x => x.ConfigurationDirectoryPath).Returns(_tempDirectory.FullName);
        return appPaths.Object;
    }
}
