using System;
using System.IO;
using Emby.Server.Implementations.AppBase;
using Emby.Server.Implementations.Serialization;
using MediaBrowser.Model.Branding;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.AppBase;

public sealed class ConfigurationHelperTests : IDisposable
{
    private readonly MyXmlSerializer _serializer = new MyXmlSerializer();
    private readonly DirectoryInfo _tempDirectory = Directory.CreateTempSubdirectory();

    public void Dispose()
    {
        _tempDirectory.Delete(true);
    }

    [Fact]
    public void GetXmlConfiguration_FileMissing_ReturnsDefaultAndCreatesFile()
    {
        var path = Path.Combine(_tempDirectory.FullName, "sub", "branding.xml");

        var result = ConfigurationHelper.GetXmlConfiguration(typeof(BrandingOptions), path, _serializer);

        var options = Assert.IsType<BrandingOptions>(result);
        Assert.Null(options.LoginDisclaimer);
        Assert.False(options.SplashscreenEnabled);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void GetXmlConfiguration_ExistingFile_DeserializesValues()
    {
        var path = Path.Combine(_tempDirectory.FullName, "branding.xml");
        var saved = new BrandingOptions
        {
            LoginDisclaimer = "Hello",
            CustomCss = "body { }",
            SplashscreenEnabled = true
        };
        _serializer.SerializeToFile(saved, path);
        var bytesBefore = File.ReadAllBytes(path);

        var result = ConfigurationHelper.GetXmlConfiguration(typeof(BrandingOptions), path, _serializer);

        var options = Assert.IsType<BrandingOptions>(result);
        Assert.Equal(saved.LoginDisclaimer, options.LoginDisclaimer);
        Assert.Equal(saved.CustomCss, options.CustomCss);
        Assert.Equal(saved.SplashscreenEnabled, options.SplashscreenEnabled);
        Assert.Equal(bytesBefore, File.ReadAllBytes(path));
    }

    [Fact]
    public void GetXmlConfiguration_CorruptFile_ReturnsDefaultAndRewritesFile()
    {
        var path = Path.Combine(_tempDirectory.FullName, "branding.xml");
        File.WriteAllText(path, "this is not xml");

        var result = ConfigurationHelper.GetXmlConfiguration(typeof(BrandingOptions), path, _serializer);

        var options = Assert.IsType<BrandingOptions>(result);
        Assert.Null(options.LoginDisclaimer);

        var reloaded = Assert.IsType<BrandingOptions>(_serializer.DeserializeFromFile(typeof(BrandingOptions), path));
        Assert.Null(reloaded.LoginDisclaimer);
        Assert.False(reloaded.SplashscreenEnabled);
    }
}
