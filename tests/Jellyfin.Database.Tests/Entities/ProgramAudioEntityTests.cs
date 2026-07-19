using System;
using Jellyfin.Database.Implementations.Entities;
using Xunit;

namespace Jellyfin.Database.Tests.Entities
{
    public class ProgramAudioEntityTests
    {
        [Theory]
        [InlineData(ProgramAudioEntity.Mono, 0)]
        [InlineData(ProgramAudioEntity.Stereo, 1)]
        [InlineData(ProgramAudioEntity.Dolby, 2)]
        [InlineData(ProgramAudioEntity.DolbyDigital, 3)]
        [InlineData(ProgramAudioEntity.Thx, 4)]
        [InlineData(ProgramAudioEntity.Atmos, 5)]
        public void Values_AreStable(ProgramAudioEntity audio, int expected)
        {
            Assert.Equal(expected, (int)audio);
        }

        [Fact]
        public void Values_HaveExpectedCount()
        {
            Assert.Equal(6, Enum.GetValues<ProgramAudioEntity>().Length);
        }
    }
}
