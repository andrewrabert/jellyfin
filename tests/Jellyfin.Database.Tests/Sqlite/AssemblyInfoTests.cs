using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Jellyfin.Database.Providers.Sqlite;
using Xunit;

namespace Jellyfin.Database.Tests.Sqlite
{
    public class AssemblyInfoTests
    {
        private static Assembly ProviderAssembly => typeof(SqliteDatabaseProvider).Assembly;

        [Fact]
        public void Assembly_IsNotComVisible()
        {
            var comVisible = ProviderAssembly.GetCustomAttribute<ComVisibleAttribute>();

            Assert.NotNull(comVisible);
            Assert.False(comVisible.Value);
        }

        [Fact]
        public void Assembly_HasEnglishNeutralResourcesLanguage()
        {
            var neutralLanguage = ProviderAssembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>();

            Assert.NotNull(neutralLanguage);
            Assert.Equal("en", neutralLanguage.CultureName);
        }

        [Fact]
        public void Assembly_ExposesInternalsToServerImplementationsTests()
        {
            var visibleTo = ProviderAssembly.GetCustomAttributes<InternalsVisibleToAttribute>()
                .Select(a => a.AssemblyName);

            Assert.Contains("Jellyfin.Server.Implementations.Tests", visibleTo);
        }

        [Fact]
        public void Assembly_HasJellyfinCompanyAndProduct()
        {
            Assert.Equal("Jellyfin Project", ProviderAssembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company);
            Assert.Equal("Jellyfin Server", ProviderAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product);
        }
    }
}
