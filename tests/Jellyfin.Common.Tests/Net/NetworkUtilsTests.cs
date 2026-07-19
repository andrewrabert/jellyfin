using System;
using System.Net;
using System.Net.Sockets;
using MediaBrowser.Common.Net;
using Xunit;

namespace Jellyfin.Common.Tests.Net
{
    public static class NetworkUtilsTests
    {
        [Theory]
        [InlineData("fe80::1", true)]
        [InlineData("febf::1", true)]
        [InlineData("fe7f::1", false)]
        [InlineData("fec0::1", false)]
        [InlineData("2001:db8::1", false)]
        [InlineData("192.168.1.1", false)]
        [InlineData("::ffff:192.168.1.1", false)]
        public static void IsIPv6LinkLocal_Valid_Success(string address, bool expected)
        {
            Assert.Equal(expected, NetworkUtils.IsIPv6LinkLocal(IPAddress.Parse(address)));
        }

        [Theory]
        [InlineData(8, "255.0.0.0")]
        [InlineData(16, "255.255.0.0")]
        [InlineData(24, "255.255.255.0")]
        [InlineData(25, "255.255.255.128")]
        [InlineData(32, "255.255.255.255")]
        public static void CidrToMask_Valid_Success(int cidr, string expected)
        {
            Assert.Equal(IPAddress.Parse(expected), NetworkUtils.CidrToMask(cidr, AddressFamily.InterNetwork));
            Assert.Equal(IPAddress.Parse(expected), NetworkUtils.CidrToMask((byte)cidr, AddressFamily.InterNetwork));
        }

        [Theory]
        [InlineData("0.0.0.0", 0)]
        [InlineData("255.0.0.0", 8)]
        [InlineData("255.255.0.0", 16)]
        [InlineData("255.255.128.0", 17)]
        [InlineData("255.255.255.0", 24)]
        [InlineData("255.255.255.255", 32)]
        [InlineData("ffff:ffff::", 32)]
        public static void MaskToCidr_Valid_Success(string mask, byte expected)
        {
            Assert.Equal(expected, NetworkUtils.MaskToCidr(IPAddress.Parse(mask)));
        }

        [Fact]
        public static void FormatIPString_Null_Empty()
        {
            Assert.Equal(string.Empty, NetworkUtils.FormatIPString(null));
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.1")]
        [InlineData("2001:db8::1", "[2001:db8::1]")]
        [InlineData("fe80::1%15", "[fe80::1]")]
        public static void FormatIPString_Valid_Success(string address, string expected)
        {
            Assert.Equal(expected, NetworkUtils.FormatIPString(IPAddress.Parse(address)));
        }

        [Theory]
        [InlineData("192.168.1.0", 24, "192.168.1.255")]
        [InlineData("10.0.0.0", 8, "10.255.255.255")]
        [InlineData("172.16.0.0", 30, "172.16.0.3")]
        public static void GetBroadcastAddress_Valid_Success(string baseAddress, int prefixLength, string expected)
        {
            var network = new IPNetwork(IPAddress.Parse(baseAddress), prefixLength);
            Assert.Equal(IPAddress.Parse(expected), NetworkUtils.GetBroadcastAddress(network));
        }

        [Theory]
        [InlineData("192.168.1.0", 24, "192.168.1.55", true)]
        [InlineData("192.168.1.0", 24, "::ffff:192.168.1.55", true)]
        [InlineData("192.168.1.0", 24, "192.168.2.1", false)]
        [InlineData("2001:db8::", 32, "2001:db8::42", true)]
        [InlineData("2001:db8::", 32, "2001:db9::42", false)]
        public static void SubnetContainsAddress_Valid_Success(string baseAddress, int prefixLength, string address, bool expected)
        {
            var network = new IPNetwork(IPAddress.Parse(baseAddress), prefixLength);
            Assert.Equal(expected, NetworkUtils.SubnetContainsAddress(network, IPAddress.Parse(address)));
        }
    }
}
