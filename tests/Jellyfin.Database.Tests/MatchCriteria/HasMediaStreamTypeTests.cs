using System.Collections.Generic;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.MatchCriteria;
using Xunit;

namespace Jellyfin.Database.Tests.MatchCriteria;

public class HasMediaStreamTypeTests
{
    [Fact]
    public void IsFolderMatchCriteria()
    {
        Assert.IsAssignableFrom<FolderMatchCriteria>(new HasMediaStreamType(MediaStreamTypeEntity.Audio, "eng"));
    }

    [Fact]
    public void PrimaryConstructor_SetsProperties()
    {
        var languages = new[] { "eng", "jpn" };

        var criteria = new HasMediaStreamType(MediaStreamTypeEntity.Subtitle, languages, true);

        Assert.Equal(MediaStreamTypeEntity.Subtitle, criteria.StreamType);
        Assert.Same(languages, criteria.Language);
        Assert.True(criteria.IsExternal);
    }

    [Fact]
    public void SingleLanguageConstructor_WrapsLanguageInCollection()
    {
        var criteria = new HasMediaStreamType(MediaStreamTypeEntity.Audio, "eng");

        var language = Assert.Single(criteria.Language);
        Assert.Equal("eng", language);
    }

    [Fact]
    public void IsExternal_DefaultsToNull()
    {
        var criteria = new HasMediaStreamType(MediaStreamTypeEntity.Audio, "eng");

        Assert.Null(criteria.IsExternal);
    }

    [Fact]
    public void Instances_WithSameLanguageCollection_AreValueEqual()
    {
        IReadOnlyCollection<string> languages = new[] { "eng" };

        var left = new HasMediaStreamType(MediaStreamTypeEntity.Audio, languages, false);
        var right = new HasMediaStreamType(MediaStreamTypeEntity.Audio, languages, false);

        Assert.Equal(left, right);
    }

    [Fact]
    public void Instances_WithDifferentStreamType_AreNotEqual()
    {
        IReadOnlyCollection<string> languages = new[] { "eng" };

        var left = new HasMediaStreamType(MediaStreamTypeEntity.Audio, languages);
        var right = new HasMediaStreamType(MediaStreamTypeEntity.Subtitle, languages);

        Assert.NotEqual(left, right);
    }
}
