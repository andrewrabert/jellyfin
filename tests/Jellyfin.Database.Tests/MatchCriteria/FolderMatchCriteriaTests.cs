using Jellyfin.Database.Implementations.MatchCriteria;
using Xunit;

namespace Jellyfin.Database.Tests.MatchCriteria;

public class FolderMatchCriteriaTests
{
    [Fact]
    public void IsAbstract()
    {
        Assert.True(typeof(FolderMatchCriteria).IsAbstract);
    }

    [Theory]
    [InlineData(typeof(HasSubtitles))]
    [InlineData(typeof(HasChapterImages))]
    [InlineData(typeof(HasMediaStreamType))]
    public void KnownCriteria_DeriveFromFolderMatchCriteria(System.Type type)
    {
        Assert.True(typeof(FolderMatchCriteria).IsAssignableFrom(type));
    }
}
