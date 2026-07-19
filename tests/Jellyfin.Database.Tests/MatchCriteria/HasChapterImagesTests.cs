using Jellyfin.Database.Implementations.MatchCriteria;
using Xunit;

namespace Jellyfin.Database.Tests.MatchCriteria;

public class HasChapterImagesTests
{
    [Fact]
    public void IsFolderMatchCriteria()
    {
        Assert.IsAssignableFrom<FolderMatchCriteria>(new HasChapterImages());
    }

    [Fact]
    public void Instances_AreValueEqual()
    {
        Assert.Equal(new HasChapterImages(), new HasChapterImages());
    }

    [Fact]
    public void NotEqualToOtherCriteriaTypes()
    {
        Assert.NotEqual<FolderMatchCriteria>(new HasChapterImages(), new HasSubtitles());
    }
}
