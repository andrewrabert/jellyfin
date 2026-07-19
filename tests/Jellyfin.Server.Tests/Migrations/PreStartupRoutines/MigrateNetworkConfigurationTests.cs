using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Emby.Server.Implementations;
using Jellyfin.Server.Migrations.PreStartupRoutines;
using MediaBrowser.Common.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Server.Tests.Migrations.PreStartupRoutines;

public sealed class MigrateNetworkConfigurationTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();
    private readonly ServerApplicationPaths _appPaths;
    private readonly string _networkConfigPath;

    public MigrateNetworkConfigurationTests()
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
    public void Perform_OldConfigPresent_MigratesValuesToNewSchema()
    {
        File.WriteAllText(
            _networkConfigPath,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <NetworkConfiguration>
              <BaseUrl>media/</BaseUrl>
              <EnableHttps>true</EnableHttps>
              <RequireHttps>true</RequireHttps>
              <HttpServerPortNumber>8100</HttpServerPortNumber>
              <HttpsPortNumber>8920</HttpsPortNumber>
              <PublicPort>80</PublicPort>
              <PublicHttpsPort>443</PublicHttpsPort>
              <EnableIPV4>true</EnableIPV4>
              <EnableIPV6>true</EnableIPV6>
              <AutoDiscovery>false</AutoDiscovery>
              <EnableRemoteAccess>false</EnableRemoteAccess>
              <KnownProxies>
                <string>10.0.0.1</string>
                <string>10.0.0.2</string>
              </KnownProxies>
              <LocalNetworkSubnets>
                <string>192.168.1.0/24</string>
              </LocalNetworkSubnets>
              <VirtualInterfaceNames>eth1*,docker0</VirtualInterfaceNames>
            </NetworkConfiguration>
            """);

        Perform();
        var newConfig = ReadNewConfig();

        Assert.Equal("/media", newConfig.BaseUrl);
        Assert.True(newConfig.EnableHttps);
        Assert.True(newConfig.RequireHttps);
        Assert.Equal(8100, newConfig.InternalHttpPort);
        Assert.Equal(8920, newConfig.InternalHttpsPort);
        Assert.Equal(80, newConfig.PublicHttpPort);
        Assert.Equal(443, newConfig.PublicHttpsPort);
        Assert.True(newConfig.EnableIPv4);
        Assert.True(newConfig.EnableIPv6);
        Assert.False(newConfig.AutoDiscovery);
        Assert.False(newConfig.EnableRemoteAccess);
        Assert.Equal(new[] { "10.0.0.1", "10.0.0.2" }, newConfig.KnownProxies);
        Assert.Equal(new[] { "192.168.1.0/24" }, newConfig.LocalNetworkSubnets);
        Assert.Equal(new[] { "eth1", "docker0" }, newConfig.VirtualInterfaceNames);
    }

    [Fact]
    public void Perform_OldDefaultVirtualInterfaceNames_MigratesToVeth()
    {
        File.WriteAllText(
            _networkConfigPath,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <NetworkConfiguration>
              <VirtualInterfaceNames>vEthernet*</VirtualInterfaceNames>
            </NetworkConfiguration>
            """);

        Perform();
        var newConfig = ReadNewConfig();

        Assert.Equal(new[] { "veth" }, newConfig.VirtualInterfaceNames);
    }

    [Fact]
    public void Perform_InvalidConfigFile_LeavesFileUntouched()
    {
        const string InvalidContent = "<not-a-network-configuration />";
        File.WriteAllText(_networkConfigPath, InvalidContent);

        Perform();

        Assert.Equal(InvalidContent, File.ReadAllText(_networkConfigPath));
    }

    private void Perform()
        => new MigrateNetworkConfiguration(_appPaths, NullLoggerFactory.Instance).Perform();

    private NetworkConfiguration ReadNewConfig()
    {
        var serializer = new XmlSerializer(typeof(NetworkConfiguration));
        using var xmlReader = XmlReader.Create(_networkConfigPath);
        return Assert.IsType<NetworkConfiguration>(serializer.Deserialize(xmlReader));
    }
}
