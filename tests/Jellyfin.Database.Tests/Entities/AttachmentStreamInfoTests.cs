using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class AttachmentStreamInfoTests
    {
        private static AttachmentStreamInfo CreateInstance()
        {
            var item = new BaseItemEntity { Id = Guid.NewGuid(), Type = "Movie" };
            return new AttachmentStreamInfo
            {
                ItemId = item.Id,
                Item = item,
                Index = 3
            };
        }

        [Fact]
        public void RequiredProperties_RoundTrip()
        {
            var attachment = CreateInstance();

            Assert.Equal(attachment.Item.Id, attachment.ItemId);
            Assert.Equal(3, attachment.Index);
        }

        [Fact]
        public void OptionalProperties_DefaultToNull()
        {
            var attachment = CreateInstance();

            Assert.Null(attachment.Codec);
            Assert.Null(attachment.CodecTag);
            Assert.Null(attachment.Comment);
            Assert.Null(attachment.Filename);
            Assert.Null(attachment.MimeType);
        }

        [Fact]
        public void OptionalProperties_RoundTrip()
        {
            var attachment = CreateInstance();
            attachment.Codec = "mjpeg";
            attachment.CodecTag = "tag";
            attachment.Comment = "comment";
            attachment.Filename = "cover.jpg";
            attachment.MimeType = "image/jpeg";

            Assert.Equal("mjpeg", attachment.Codec);
            Assert.Equal("tag", attachment.CodecTag);
            Assert.Equal("comment", attachment.Comment);
            Assert.Equal("cover.jpg", attachment.Filename);
            Assert.Equal("image/jpeg", attachment.MimeType);
        }
    }
}
