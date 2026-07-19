using System;
using System.IO;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Moq;
using Xunit;

namespace Jellyfin.Common.Tests.Plugins
{
    public class BasePluginOfTTests
    {
        private readonly Mock<IApplicationPaths> _applicationPaths;
        private readonly Mock<IXmlSerializer> _xmlSerializer;

        public BasePluginOfTTests()
        {
            var tempPath = Path.GetTempPath();
            _applicationPaths = new Mock<IApplicationPaths>();
            _applicationPaths.Setup(p => p.PluginsPath).Returns(Path.Combine(tempPath, "plugins"));
            _applicationPaths.Setup(p => p.PluginConfigurationsPath).Returns(Path.Combine(tempPath, "plugin-configurations"));
            _xmlSerializer = new Mock<IXmlSerializer>();
        }

        [Fact]
        public void ConfigurationFilePath_FromAssemblyName_Success()
        {
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);

            Assert.Equal("Jellyfin.Common.Tests.xml", plugin.ConfigurationFileName);
            Assert.Equal(
                Path.Combine(_applicationPaths.Object.PluginConfigurationsPath, "Jellyfin.Common.Tests.xml"),
                plugin.ConfigurationFilePath);
        }

        [Fact]
        public void Configuration_DeserializationSucceeds_ReturnsDeserialized()
        {
            var config = new TestPluginConfiguration();
            _xmlSerializer
                .Setup(s => s.DeserializeFromFile(typeof(TestPluginConfiguration), It.IsAny<string>()))
                .Returns(config);
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);

            Assert.Same(config, plugin.Configuration);
            _xmlSerializer.Verify(s => s.SerializeToFile(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Configuration_DeserializationFails_CreatesAndSavesDefault()
        {
            _xmlSerializer
                .Setup(s => s.DeserializeFromFile(typeof(TestPluginConfiguration), It.IsAny<string>()))
                .Throws<FileNotFoundException>();
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);

            var config = plugin.Configuration;

            Assert.NotNull(config);
            _xmlSerializer.Verify(s => s.SerializeToFile(config, plugin.ConfigurationFilePath), Times.Once);
        }

        [Fact]
        public void UpdateConfiguration_Valid_SavesAndRaisesEvent()
        {
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);
            var newConfig = new TestPluginConfiguration();
            BasePluginConfiguration? raisedConfig = null;
            plugin.ConfigurationChanged += (_, config) => raisedConfig = config;

            plugin.UpdateConfiguration(newConfig);

            Assert.Same(newConfig, plugin.Configuration);
            Assert.Same(newConfig, raisedConfig);
            _xmlSerializer.Verify(s => s.SerializeToFile(newConfig, plugin.ConfigurationFilePath), Times.Once);
        }

        [Fact]
        public void UpdateConfiguration_Null_ThrowsArgumentNullException()
        {
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);

            Assert.Throws<ArgumentNullException>(() => plugin.UpdateConfiguration(null!));
        }

        [Fact]
        public void GetPluginInfo_Valid_ContainsConfigurationFileName()
        {
            var plugin = new TestPlugin(_applicationPaths.Object, _xmlSerializer.Object);

            var info = plugin.GetPluginInfo();

            Assert.Equal("Test Plugin", info.Name);
            Assert.Equal("Jellyfin.Common.Tests.xml", info.ConfigurationFileName);
        }

        private sealed class TestPluginConfiguration : BasePluginConfiguration
        {
        }

        private sealed class TestPlugin : BasePlugin<TestPluginConfiguration>
        {
            public TestPlugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
                : base(applicationPaths, xmlSerializer)
            {
            }

            public override string Name => "Test Plugin";
        }
    }
}
