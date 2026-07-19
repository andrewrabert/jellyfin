using System;
using System.Collections.Generic;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class BaseItemEntityTests
    {
        private static BaseItemEntity CreateInstance()
            => new BaseItemEntity { Id = Guid.NewGuid(), Type = "MediaBrowser.Controller.Entities.Movies.Movie" };

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var id = Guid.NewGuid();
            var entity = new BaseItemEntity { Id = id, Type = "Movie" };

            Assert.Equal(id, entity.Id);
            Assert.Equal("Movie", entity.Type);
        }

        [Fact]
        public void OptionalProperties_HaveExpectedDefaults()
        {
            var entity = CreateInstance();

            Assert.Null(entity.Data);
            Assert.Null(entity.Path);
            Assert.Null(entity.Name);
            Assert.Null(entity.StartDate);
            Assert.Null(entity.EndDate);
            Assert.Null(entity.ChannelId);
            Assert.Null(entity.CommunityRating);
            Assert.Null(entity.IndexNumber);
            Assert.Null(entity.RunTimeTicks);
            Assert.Null(entity.ExtraType);
            Assert.Null(entity.Audio);
            Assert.Null(entity.ParentId);
            Assert.Null(entity.OwnerId);
            Assert.Null(entity.Owner);
            Assert.Null(entity.Extras);
            Assert.Null(entity.Peoples);
            Assert.Null(entity.UserData);
            Assert.Null(entity.ItemValues);
            Assert.Null(entity.MediaStreams);
            Assert.Null(entity.Chapters);
            Assert.Null(entity.Provider);
            Assert.Null(entity.Parents);
            Assert.Null(entity.Children);
            Assert.Null(entity.DirectChildren);
            Assert.Null(entity.LockedFields);
            Assert.Null(entity.TrailerTypes);
            Assert.Null(entity.Images);
            Assert.Null(entity.LinkedChildEntities);
            Assert.Null(entity.LinkedChildOfEntities);
            Assert.False(entity.IsMovie);
            Assert.False(entity.IsSeries);
            Assert.False(entity.IsFolder);
            Assert.False(entity.IsLocked);
            Assert.False(entity.IsRepeat);
            Assert.False(entity.IsVirtualItem);
            Assert.False(entity.IsInMixedFolder);
        }

        [Fact]
        public void ScalarProperties_RoundTrip()
        {
            var now = DateTime.UtcNow;
            var entity = CreateInstance();

            entity.Name = "The Movie";
            entity.Path = "/media/movie.mkv";
            entity.IsMovie = true;
            entity.CommunityRating = 8.4f;
            entity.CriticRating = 91f;
            entity.IndexNumber = 2;
            entity.ParentIndexNumber = 1;
            entity.ProductionYear = 2020;
            entity.RunTimeTicks = 12345L;
            entity.DateCreated = now;
            entity.ExtraType = BaseItemExtraType.Trailer;
            entity.Audio = ProgramAudioEntity.Stereo;
            entity.Width = 1920;
            entity.Height = 1080;
            entity.Size = 1_000_000L;
            entity.TotalBitrate = 5000;

            Assert.Equal("The Movie", entity.Name);
            Assert.Equal("/media/movie.mkv", entity.Path);
            Assert.True(entity.IsMovie);
            Assert.Equal(8.4f, entity.CommunityRating);
            Assert.Equal(91f, entity.CriticRating);
            Assert.Equal(2, entity.IndexNumber);
            Assert.Equal(1, entity.ParentIndexNumber);
            Assert.Equal(2020, entity.ProductionYear);
            Assert.Equal(12345L, entity.RunTimeTicks);
            Assert.Equal(now, entity.DateCreated);
            Assert.Equal(BaseItemExtraType.Trailer, entity.ExtraType);
            Assert.Equal(ProgramAudioEntity.Stereo, entity.Audio);
            Assert.Equal(1920, entity.Width);
            Assert.Equal(1080, entity.Height);
            Assert.Equal(1_000_000L, entity.Size);
            Assert.Equal(5000, entity.TotalBitrate);
        }

        [Fact]
        public void NavigationProperties_RoundTrip()
        {
            var entity = CreateInstance();
            var owner = CreateInstance();
            var extras = new List<BaseItemEntity> { CreateInstance() };

            entity.OwnerId = owner.Id;
            entity.Owner = owner;
            entity.Extras = extras;
            entity.DirectParent = owner;
            entity.ParentId = owner.Id;

            Assert.Equal(owner.Id, entity.OwnerId);
            Assert.Same(owner, entity.Owner);
            Assert.Same(extras, entity.Extras);
            Assert.Same(owner, entity.DirectParent);
            Assert.Equal(owner.Id, entity.ParentId);
        }
    }
}
