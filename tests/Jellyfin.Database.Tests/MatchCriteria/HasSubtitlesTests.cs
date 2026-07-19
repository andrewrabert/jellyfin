using Jellyfin.Database.Implementations.MatchCriteria;
using Xunit;

namespace Jellyfin.Database.Tests.MatchCriteria;

public class HasSubtitlesTests
{
    [Fact]
    public void IsFolderMatchCriteria()
    {
        Assert.IsAssignableFrom<FolderMatchCriteria>(new HasSubtitles());
    }

    [Fact]
    public void Instances_AreValueEqual()
    {
        Assert.Equal(new HasSubtitles(), new HasSubtitles());
    }

    [Fact]
    public void NotEqualToOtherCriteriaTypes()
    {
        Assert.NotEqual<FolderMatchCriteria>(new HasSubtitles(), new HasChapterImages());
    }
}
