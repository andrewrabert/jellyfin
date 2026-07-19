using System;
using System.IO;
using System.Xml.Linq;
using Emby.Server.Implementations;
using Jellyfin.Server.Migrations.PreStartupRoutines;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Jellyfin.Server.Tests.Migrations.PreStartupRoutines;

public sealed class RenameEnableGroupingIntoCollectionsTests : IDisposable
{
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();
    private readonly ServerApplicationPaths _appPaths;
    private readonly string _systemConfigPath;

    public RenameEnableGroupingIntoCollectionsTests()
    {
        _appPaths = new ServerApplicationPaths(
            _tempDirectory.FullName,
            Path.Combine(_tempDirectory.FullName, "log"),
            Path.Combine(_tempDirectory.FullName, "config"),
            Path.Combine(_tempDirectory.FullName, "cache"),
            Path.Combine(_tempDirectory.FullName, "web"));
        Directory.CreateDirectory(_appPaths.ConfigurationDirectoryPath);
        _systemConfigPath = Path.Combine(_appPaths.ConfigurationDirectoryPath, "system.xml");
    }

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    [Fact]
    public void Perform_OldTagPresent_RenamesTagAndKeepsValue()
    {
        File.WriteAllText(
            _systemConfigPath,
            """
            <?xml version="1.0" encoding="utf-8"?>
            <ServerConfiguration>
              <EnableGroupingIntoCollections>true</EnableGroupingIntoCollections>
              <EnableFolderView>false</EnableFolderView>
            </ServerConfiguration>
            """);

        Perform();

        var root = XDocument.Load(_systemConfigPath).Root;
        Assert.NotNull(root);
        Assert.Null(root.Element("EnableGroupingIntoCollections"));
        Assert.Equal("true", root.Element("EnableGroupingMoviesIntoCollections")?.Value);
        Assert.Equal("false", root.Element("EnableFolderView")?.Value);
    }

    [Fact]
    public void Perform_OldTagAbsent_LeavesFileUntouched()
    {
        const string Content = "<ServerConfiguration><EnableFolderView>false</EnableFolderView></ServerConfiguration>";
        File.WriteAllText(_systemConfigPath, Content);

        Perform();

        Assert.Equal(Content, File.ReadAllText(_systemConfigPath));
    }

    [Fact]
    public void Perform_ConfigFileMissing_DoesNotThrow()
    {
        Perform();

        Assert.False(File.Exists(_systemConfigPath));
    }

    private void Perform()
        => new RenameEnableGroupingIntoCollections(_appPaths, NullLoggerFactory.Instance).Perform();
}
