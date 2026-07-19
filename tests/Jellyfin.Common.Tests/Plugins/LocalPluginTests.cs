using System;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Xunit;

namespace Jellyfin.Common.Tests.Plugins
{
    public class LocalPluginTests
    {
        [Fact]
        public void Version_FromManifest_Parsed()
        {
            var plugin = CreatePlugin("Test Plugin", Guid.NewGuid(), "1.2.3.4");

            Assert.Equal(new Version(1, 2, 3, 4), plugin.Version);
        }

        [Theory]
        [InlineData(true, PluginStatus.Active, true)]
        [InlineData(true, PluginStatus.Restart, true)]
        [InlineData(true, PluginStatus.Disabled, false)]
        [InlineData(true, PluginStatus.Malfunctioned, false)]
        [InlineData(false, PluginStatus.Active, false)]
        public void IsEnabledAndSupported_Valid_Success(bool isSupported, PluginStatus status, bool expected)
        {
            var plugin = CreatePlugin("Test Plugin", Guid.NewGuid(), "1.0.0.0", status, isSupported);

            Assert.Equal(expected, plugin.IsEnabledAndSupported);
        }

        [Fact]
        public void Compare_DifferentNames_ByName()
        {
            var a = CreatePlugin("Alpha", Guid.NewGuid(), "1.0.0.0");
            var b = CreatePlugin("beta", Guid.NewGuid(), "1.0.0.0");

            Assert.True(LocalPlugin.Compare(a, b) < 0);
            Assert.True(LocalPlugin.Compare(b, a) > 0);
        }

        [Fact]
        public void Compare_SameNameAndId_ByVersion()
        {
            var id = Guid.NewGuid();
            var older = CreatePlugin("Test Plugin", id, "1.0.0.0");
            var newer = CreatePlugin("Test Plugin", id, "2.0.0.0");

            Assert.True(LocalPlugin.Compare(older, newer) < 0);
            Assert.True(LocalPlugin.Compare(newer, older) > 0);
        }

        [Fact]
        public void Compare_SameNameDifferentId_ById()
        {
            var idA = new Guid("00000000-0000-0000-0000-000000000001");
            var idB = new Guid("00000000-0000-0000-0000-000000000002");
            var a = CreatePlugin("Test Plugin", idA, "1.0.0.0");
            var b = CreatePlugin("Test Plugin", idB, "1.0.0.0");

            Assert.NotEqual(0, LocalPlugin.Compare(a, b));
        }

        [Fact]
        public void Compare_Null_ThrowsArgumentNullException()
        {
            var plugin = CreatePlugin("Test Plugin", Guid.NewGuid(), "1.0.0.0");

            Assert.Throws<ArgumentNullException>(() => LocalPlugin.Compare(plugin, null!));
            Assert.Throws<ArgumentNullException>(() => LocalPlugin.Compare(null!, plugin));
        }

        [Fact]
        public void Equals_SamePluginDifferentCase_True()
        {
            var id = Guid.NewGuid();
            var a = CreatePlugin("Test Plugin", id, "1.0.0.0");
            var b = CreatePlugin("TEST PLUGIN", id, "1.0.0.0");

            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentIdOrVersion_False()
        {
            var id = Guid.NewGuid();
            var plugin = CreatePlugin("Test Plugin", id, "1.0.0.0");

            Assert.False(plugin.Equals(CreatePlugin("Test Plugin", Guid.NewGuid(), "1.0.0.0")));
            Assert.False(plugin.Equals(CreatePlugin("Test Plugin", id, "2.0.0.0")));
            Assert.False(plugin.Equals(null));
        }

        [Fact]
        public void GetPluginInfo_NoInstance_FromManifest()
        {
            var id = Guid.NewGuid();
            var plugin = CreatePlugin("Test Plugin", id, "1.2.3.4", PluginStatus.Disabled);
            plugin.Manifest.Description = "A test plugin.";

            var info = plugin.GetPluginInfo();

            Assert.Equal("Test Plugin", info.Name);
            Assert.Equal(id, info.Id);
            Assert.Equal(new Version(1, 2, 3, 4), info.Version);
            Assert.Equal("A test plugin.", info.Description);
            Assert.Equal(PluginStatus.Disabled, info.Status);
            Assert.False(info.HasImage);
        }

        [Fact]
        public void GetPluginInfo_WithImagePath_HasImage()
        {
            var plugin = CreatePlugin("Test Plugin", Guid.NewGuid(), "1.0.0.0");
            plugin.Manifest.ImagePath = "image.png";

            Assert.True(plugin.GetPluginInfo().HasImage);
        }

        private static LocalPlugin CreatePlugin(string name, Guid id, string version, PluginStatus status = PluginStatus.Active, bool isSupported = true)
        {
            var manifest = new PluginManifest
            {
                Name = name,
                Id = id,
                Version = version,
                Status = status
            };

            return new LocalPlugin("/plugins/test", isSupported, manifest);
        }
    }
}
