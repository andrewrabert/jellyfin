using System;
using System.IO;
using System.Text;
using Emby.Server.Implementations.Serialization;
using MediaBrowser.Model.Branding;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Serialization;

public class MyXmlSerializerTests
{
    private readonly MyXmlSerializer _sut = new MyXmlSerializer();

    [Fact]
    public void SerializeToStream_ThenDeserializeFromStream_RoundTrips()
    {
        var options = new BrandingOptions
        {
            LoginDisclaimer = "Disclaimer & <special> \"chars\"",
            CustomCss = ".foo { color: red; }",
            SplashscreenEnabled = true,
            SplashscreenLocation = "/some/path.png"
        };

        using var stream = new MemoryStream();
        _sut.SerializeToStream(options, stream);
        stream.Position = 0;

        var result = Assert.IsType<BrandingOptions>(_sut.DeserializeFromStream(typeof(BrandingOptions), stream));

        Assert.Equal(options.LoginDisclaimer, result.LoginDisclaimer);
        Assert.Equal(options.CustomCss, result.CustomCss);
        Assert.Equal(options.SplashscreenEnabled, result.SplashscreenEnabled);
        Assert.Equal(options.SplashscreenLocation, result.SplashscreenLocation);
    }

    [Fact]
    public void SerializeToFile_ThenDeserializeFromFile_RoundTrips()
    {
        var options = new BrandingOptions
        {
            LoginDisclaimer = "Hello"
        };

        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            _sut.SerializeToFile(options, file);

            var result = Assert.IsType<BrandingOptions>(_sut.DeserializeFromFile(typeof(BrandingOptions), file));

            Assert.Equal(options.LoginDisclaimer, result.LoginDisclaimer);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void DeserializeFromBytes_ValidXml_ReturnsObject()
    {
        const string Xml = "<?xml version=\"1.0\"?><BrandingOptions><LoginDisclaimer>Hi</LoginDisclaimer><SplashscreenEnabled>true</SplashscreenEnabled></BrandingOptions>";

        var result = Assert.IsType<BrandingOptions>(_sut.DeserializeFromBytes(typeof(BrandingOptions), Encoding.UTF8.GetBytes(Xml)));

        Assert.Equal("Hi", result.LoginDisclaimer);
        Assert.True(result.SplashscreenEnabled);
    }

    [Fact]
    public void DeserializeFromFile_MissingFile_ThrowsWithFilenameInData()
    {
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        var ex = Assert.ThrowsAny<Exception>(() => _sut.DeserializeFromFile(typeof(BrandingOptions), file));

        Assert.Equal(file, ex.Data["Filename"]);
    }
}
