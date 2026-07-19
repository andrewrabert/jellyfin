using System;
using MediaBrowser.Common.Extensions;
using Xunit;

namespace Jellyfin.Common.Tests.Extensions
{
    public static class BaseExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("Jellyfin", "Jellyfin")]
        [InlineData("<b>Jellyfin</b>", "Jellyfin")]
        [InlineData("  <p>The Free Software Media System</p>  ", "The Free Software Media System")]
        [InlineData("<a href=\"https://jellyfin.org\">Jellyfin</a>", "Jellyfin")]
        [InlineData("Line one<br/>Line two", "Line oneLine two")]
        [InlineData("<div\nclass=\"a\">Multiline tag</div>", "Multiline tag")]
        [InlineData("1 < 2", "1 < 2")]
        public static void StripHtml_Valid_Success(string input, string expected)
        {
            Assert.Equal(expected, input.StripHtml());
        }

        [Theory]
        [InlineData("test", "2e9e05c8-41c7-599f-0e79-d7f1b774bfe6")]
        [InlineData("Jellyfin", "aa9132af-ed08-5901-05eb-68cb89645bda")]
        public static void GetMD5_Valid_Success(string input, string expected)
        {
            Assert.Equal(new Guid(expected), input.GetMD5());
        }

        [Fact]
        public static void GetMD5_SameInput_SameResult()
        {
            Assert.Equal("some input".GetMD5(), "some input".GetMD5());
            Assert.NotEqual("some input".GetMD5(), "Some Input".GetMD5());
        }
    }
}
