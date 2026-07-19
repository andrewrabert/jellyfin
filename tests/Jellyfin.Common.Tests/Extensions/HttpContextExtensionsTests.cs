using System.Net;
using MediaBrowser.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Jellyfin.Common.Tests.Extensions
{
    public static class HttpContextExtensionsTests
    {
        [Fact]
        public static void IsLocal_BothAddressesNull_True()
        {
            var context = new DefaultHttpContext();
            context.Connection.LocalIpAddress = null;
            context.Connection.RemoteIpAddress = null;

            Assert.True(context.IsLocal());
        }

        [Fact]
        public static void IsLocal_SameAddress_True()
        {
            var context = new DefaultHttpContext();
            context.Connection.LocalIpAddress = IPAddress.Parse("192.168.1.10");
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.10");

            Assert.True(context.IsLocal());
        }

        [Fact]
        public static void IsLocal_DifferentAddress_False()
        {
            var context = new DefaultHttpContext();
            context.Connection.LocalIpAddress = IPAddress.Parse("192.168.1.10");
            context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.11");

            Assert.False(context.IsLocal());
        }

        [Fact]
        public static void GetNormalizedRemoteIP_NoAddress_Loopback()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = null;

            Assert.Equal(IPAddress.Loopback, context.GetNormalizedRemoteIP());
        }

        [Fact]
        public static void GetNormalizedRemoteIP_MappedIPv6_ReturnsIPv4()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("::ffff:10.0.0.5");

            Assert.Equal(IPAddress.Parse("10.0.0.5"), context.GetNormalizedRemoteIP());
        }

        [Fact]
        public static void GetNormalizedRemoteIP_IPv6_Unchanged()
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("2001:db8::1");

            Assert.Equal(IPAddress.Parse("2001:db8::1"), context.GetNormalizedRemoteIP());
        }
    }
}
