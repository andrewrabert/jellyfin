using System;
using System.IO;
using System.Xml.Linq;
using Emby.Server.Implementations;
using Jellyfin.Server.Migrations.PreStartupRoutines;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Server.Tests.Migrations.PreStartupRoutines;

public sealed class CreateNetworkConfigurationTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();
    private readonly ServerApplicationPaths _appPaths;
    private readonly string _networkConfigPath;

    public CreateNetworkConfigurationTests()
    {
        _appPaths = new ServerApplicationPaths(
            _tempDirectory.FullName,
            Path.Combine(_tempDirectory.FullName, "log"),
            Path.Combine(_tempDirectory.FullName, "config"),
            Path.Combine(_tempDirectory.FullName, "cache"),
            Path.Combine(_tempDirectory.FullName, "web"));
        Directory.CreateDirectory(_appPaths.ConfigurationDirectoryPath);
        _networkConfigPath = Path.Combine(_appPaths.ConfigurationDirectoryPath, "network.xml");
    }

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    [Fact]
    public void Perform_NetworkSettingsInSystemConfig_ExtractsThemToNetworkConfig()
    {
        File.WriteAllText(
            _appPaths.SystemConfigurationFilePath,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ServerConfiguration>
              <BaseUrl>jellyfin</BaseUrl>
              <EnableHttps>true</EnableHttps>
              <HttpServerPortNumber>8100</HttpServerPortNumber>
              <EnableRemoteAccess>false</EnableRemoteAccess>
              <KnownProxies>
                <string>10.0.0.1</string>
              </KnownProxies>
            </ServerConfiguration>
            """);

        Perform();

        var document = XDocument.Load(_networkConfigPath);
        var root = document.Root;
        Assert.NotNull(root);
        Assert.Equal("NetworkConfiguration", root.Name.LocalName);
        Assert.Equal("/jellyfin", root.Element("BaseUrl")?.Value);
        Assert.Equal("true", root.Element("EnableHttps")?.Value);
        Assert.Equal("8100", root.Element("HttpServerPortNumber")?.Value);
        Assert.Equal("false", root.Element("EnableRemoteAccess")?.Value);
        Assert.Equal("10.0.0.1", root.Element("KnownProxies")?.Element("string")?.Value);
    }

    [Fact]
    public void Perform_NetworkConfigAlreadyExists_LeavesFileUntouched()
    {
        const string ExistingContent = "<NetworkConfiguration />";
        File.WriteAllText(_networkConfigPath, ExistingContent);

        Perform();

        Assert.Equal(ExistingContent, File.ReadAllText(_networkConfigPath));
    }

    private void Perform()
        => new CreateNetworkConfiguration(_appPaths, NullLoggerFactory.Instance).Perform();
}
