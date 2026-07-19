using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class MediaStreamInfoTests
    {
        private static MediaStreamInfo CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            return new MediaStreamInfo
            {
                ItemId = item.Id,
                Item = item,
                StreamType = MediaStreamTypeEntity.Video
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var stream = CreateInstance();

            Assert.Equal(stream.Item.Id, stream.ItemId);
            Assert.Equal(MediaStreamTypeEntity.Video, stream.StreamType);
        }

        [Fact]
        public void OptionalProperties_HaveExpectedDefaults()
        {
            var stream = CreateInstance();

            Assert.Equal(0, stream.StreamIndex);
            Assert.Null(stream.Codec);
            Assert.Null(stream.Language);
            Assert.Null(stream.ChannelLayout);
            Assert.Null(stream.Profile);
            Assert.Null(stream.AspectRatio);
            Assert.Null(stream.Path);
            Assert.Null(stream.IsInterlaced);
            Assert.Null(stream.BitRate);
            Assert.Null(stream.Channels);
            Assert.Null(stream.SampleRate);
            Assert.False(stream.IsDefault);
            Assert.False(stream.IsForced);
            Assert.False(stream.IsExternal);
            Assert.False(stream.IsOriginal);
            Assert.Null(stream.Height);
            Assert.Null(stream.Width);
            Assert.Null(stream.AverageFrameRate);
            Assert.Null(stream.RealFrameRate);
            Assert.Null(stream.Level);
            Assert.Null(stream.PixelFormat);
            Assert.Null(stream.BitDepth);
            Assert.Null(stream.IsAnamorphic);
            Assert.Null(stream.IsAvc);
            Assert.Null(stream.IsHearingImpaired);
            Assert.Null(stream.Rotation);
            Assert.Null(stream.KeyFrames);
            Assert.Null(stream.Hdr10PlusPresentFlag);
            Assert.Null(stream.DvProfile);
        }

        [Fact]
        public void Properties_RoundTrip()
        {
            var stream = CreateInstance();

            stream.StreamIndex = 2;
            stream.Codec = "h264";
            stream.Language = "eng";
            stream.Profile = "High";
            stream.BitRate = 8_000_000;
            stream.Channels = 6;
            stream.SampleRate = 48000;
            stream.IsDefault = true;
            stream.IsForced = true;
            stream.IsExternal = true;
            stream.IsOriginal = true;
            stream.Width = 1920;
            stream.Height = 1080;
            stream.AverageFrameRate = 23.976f;
            stream.Level = 4.1f;
            stream.PixelFormat = "yuv420p";
            stream.BitDepth = 10;
            stream.IsAnamorphic = false;
            stream.IsAvc = true;
            stream.IsHearingImpaired = false;
            stream.Rotation = 90;
            stream.Hdr10PlusPresentFlag = true;

            Assert.Equal(2, stream.StreamIndex);
            Assert.Equal("h264", stream.Codec);
            Assert.Equal("eng", stream.Language);
            Assert.Equal("High", stream.Profile);
            Assert.Equal(8_000_000, stream.BitRate);
            Assert.Equal(6, stream.Channels);
            Assert.Equal(48000, stream.SampleRate);
            Assert.True(stream.IsDefault);
            Assert.True(stream.IsForced);
            Assert.True(stream.IsExternal);
            Assert.True(stream.IsOriginal);
            Assert.Equal(1920, stream.Width);
            Assert.Equal(1080, stream.Height);
            Assert.Equal(23.976f, stream.AverageFrameRate);
            Assert.Equal(4.1f, stream.Level);
            Assert.Equal("yuv420p", stream.PixelFormat);
            Assert.Equal(10, stream.BitDepth);
            Assert.False(stream.IsAnamorphic);
            Assert.True(stream.IsAvc);
            Assert.False(stream.IsHearingImpaired);
            Assert.Equal(90, stream.Rotation);
            Assert.True(stream.Hdr10PlusPresentFlag);
        }
    }
}
