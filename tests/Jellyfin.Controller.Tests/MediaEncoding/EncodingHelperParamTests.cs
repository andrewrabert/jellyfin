using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.IO;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Entities;
using Moq;
using Xunit;

using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Jellyfin.Controller.Tests.MediaEncoding;

public class EncodingHelperParamTests
{
    [Theory]
    [InlineData("mkv", "matroska")]
    [InlineData("MKV", "matroska")]
    [InlineData("ts", "mpegts")]
    [InlineData("mp4", "mp4")]
    [InlineData("avi", "avi")]
    [InlineData("m2ts", null)]
    [InlineData("wmv", null)]
    [InlineData("vob", null)]
    [InlineData("mpeg", null)]
    [InlineData("m4v", null)]
    [InlineData("strm", null)]
    [InlineData("iso", null)]
    [InlineData("", null)]
    [InlineData("bad container!", null)]
    public void GetInputFormat_MapsContainerToFfmpegFormat(string container, string? expected)
    {
        Assert.Equal(expected, EncodingHelper.GetInputFormat(container));
    }

    [Theory]
    [InlineData("ogg", "opus")]
    [InlineData("webm", "opus")]
    [InlineData("webma", "opus")]
    [InlineData("mp4", "aac")]
    [InlineData("mkv", "aac")]
    [InlineData("M4A", "aac")]
    [InlineData("ts", "mp3")]
    [InlineData("avi", "mp3")]
    [InlineData("flac", "flac")]
    [InlineData("WAV", "wav")]
    [InlineData("", "aac")]
    [InlineData("   ", "aac")]
    public void InferAudioCodec_MapsContainerToCodec(string container, string expected)
    {
        Assert.Equal(expected, CreateHelper().InferAudioCodec(container));
    }

    [Theory]
    [InlineData("/media/movie.asf", "wmv")]
    [InlineData("/media/movie.webm", "vp8")]
    [InlineData("/media/movie.ogg", "theora")]
    [InlineData("/media/movie.ogv", "theora")]
    [InlineData("/media/stream.m3u8", "h264")]
    [InlineData("/media/stream.TS", "h264")]
    [InlineData("/media/movie.mp4", "copy")]
    [InlineData("/media/movie", "copy")]
    public void InferVideoCodec_MapsExtensionToCodec(string url, string expected)
    {
        Assert.Equal(expected, CreateHelper().InferVideoCodec(url));
    }

    [Theory]
    [InlineData("copy", true)]
    [InlineData("COPY", true)]
    [InlineData("h264", false)]
    [InlineData("", false)]
    public void IsCopyCodec_MatchesCaseInsensitively(string codec, bool expected)
    {
        Assert.Equal(expected, EncodingHelper.IsCopyCodec(codec));
    }

    [Theory]
    [InlineData("mp4", ".mp4")]
    [InlineData("ts", ".ts")]
    [InlineData(null, ".ts")]
    [InlineData("  ", ".ts")]
    public void GetSegmentFileExtension_DefaultsToTs(string? segmentContainer, string expected)
    {
        Assert.Equal(expected, EncodingHelper.GetSegmentFileExtension(segmentContainer!));
    }

    [Theory]
    [InlineData(10_000_000, "h264", "h264", 10_000_000)] // no efficiency delta, no low-bitrate boost
    [InlineData(10_000_000, "hevc", "h264", 16_666_667)] // hevc source needs more bits as h264
    [InlineData(10_000_000, "h264", "hevc", 10_000_000)] // never scale below the request
    [InlineData(10_000_000, "av1", "h264", 20_000_000)]
    [InlineData(500_000, "h264", "h264", 2_000_000)] // low-bitrate boost x4
    [InlineData(1_000_000, "h264", "h264", 3_000_000)] // low-bitrate boost x3
    [InlineData(2_000_000, "h264", "h264", 5_000_000)] // low-bitrate boost x2.5
    [InlineData(3_000_000, "h264", "h264", 6_000_000)] // low-bitrate boost x2
    [InlineData(40_000_000, "av1", "h264", 40_000_000)] // no scaling at or above 30Mbps
    public void ScaleBitrate_AppliesCodecEfficiencyAndFloors(int bitrate, string inputCodec, string outputCodec, int expected)
    {
        Assert.Equal(expected, EncodingHelper.ScaleBitrate(bitrate, inputCodec, outputCodec));
    }

    [Fact]
    public void GetNumberOfThreads_AutomaticWhenUnset()
    {
        var options = new EncodingOptions(); // EncodingThreadCount defaults to -1
        Assert.Equal(0, EncodingHelper.GetNumberOfThreads(null, options, "libx264"));
    }

    [Fact]
    public void GetNumberOfThreads_CappedByProcessorCount()
    {
        var options = new EncodingOptions { EncodingThreadCount = int.MaxValue };
        Assert.Equal(Environment.ProcessorCount, EncodingHelper.GetNumberOfThreads(null, options, "libx264"));
    }

    [Fact]
    public void GetNumberOfThreads_CpuCoreLimitOverridesOptions()
    {
        var options = new EncodingOptions { EncodingThreadCount = -1 };
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            BaseRequest = new BaseEncodingJobOptions { CpuCoreLimit = 1 }
        };

        Assert.Equal(1, EncodingHelper.GetNumberOfThreads(state, options, "libx264"));
    }

    [Theory]
    [InlineData("h264", "153", "51")]
    [InlineData("h264", "-1", "51")]
    [InlineData("h264", "41", "41")]
    [InlineData("hevc", "153", "150")]
    [InlineData("hevc", "120", "120")]
    [InlineData("av1", "19", "15")]
    [InlineData("av1", "8", "8")]
    [InlineData("mpeg2video", "9999", "9999")]
    [InlineData("h264", "high", null)]
    public void NormalizeTranscodingLevel_ClampsPerCodec(string outputVideoCodec, string level, string? expected)
    {
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            OutputVideoCodec = outputVideoCodec,
            VideoStream = new MediaStream { Type = MediaStreamType.Video, Codec = "h264" },
            BaseRequest = new BaseEncodingJobOptions()
        };

        Assert.Equal(expected, EncodingHelper.NormalizeTranscodingLevel(state, level));
    }

    [Theory]
    [InlineData("-1", "5.1", " -fps_mode auto")]
    [InlineData("0", "6.0", " -fps_mode passthrough")]
    [InlineData("1", "5.1", " -fps_mode cfr")]
    [InlineData("2", "5.1", " -fps_mode vfr")]
    [InlineData("7", "5.1", "")]
    [InlineData("cfr", "5.1", "")]
    [InlineData("2", "4.4", " -vsync 2")]
    [InlineData("passthrough", "4.4", " -vsync passthrough")]
    [InlineData("", "5.1", "")]
    public void GetVideoSyncOption_DependsOnEncoderVersion(string videoSync, string encoderVersion, string expected)
    {
        Assert.Equal(expected, EncodingHelper.GetVideoSyncOption(videoSync, Version.Parse(encoderVersion)));
    }

    [Fact]
    public void FindIndex_CountsOnlyStreamsWithSamePath()
    {
        var internal1 = new MediaStream { Index = 0 };
        var external1 = new MediaStream { Index = 1, Path = "/media/movie.mks" };
        var external2 = new MediaStream { Index = 2, Path = "/media/movie.en.srt" };
        var external3 = new MediaStream { Index = 3, Path = "/media/movie.mks" };
        var streams = new List<MediaStream> { internal1, external1, external2, external3 };

        Assert.Equal(0, EncodingHelper.FindIndex(streams, external1));
        Assert.Equal(0, EncodingHelper.FindIndex(streams, external2));
        Assert.Equal(1, EncodingHelper.FindIndex(streams, external3));
    }

    [Fact]
    public void FindIndex_StreamNotInList_ReturnsMinusOne()
    {
        var streams = new List<MediaStream> { new MediaStream { Index = 0, Path = "/media/movie.mkv" } };
        var missing = new MediaStream { Index = 5, Path = "/media/other.mkv" };

        Assert.Equal(-1, EncodingHelper.FindIndex(streams, missing));
    }

    [Theory]
    [InlineData("aac", true)]
    [InlineData("AAC", true)]
    [InlineData("aac_latm", true)]
    [InlineData("mp3", false)]
    [InlineData(null, false)]
    public void IsAAC_MatchesCodecSubstring(string? codec, bool expected)
    {
        Assert.Equal(expected, EncodingHelper.IsAAC(new MediaStream { Codec = codec }));
    }

    [Fact]
    public void GetAudioBitrateParam_NullAudioStream_ReturnsNull()
    {
        Assert.Null(CreateHelper().GetAudioBitrateParam(128000, "aac", null!, 2));
    }

    [Theory]
    [InlineData(null, "aac", 6, null, 640000)] // 5.1 passthrough channel count
    [InlineData(2_000_000, "aac", 6, 6, 640000)] // capped at codec maximum
    [InlineData(100_000, "aac", 6, 6, 100_000)] // requested bitrate wins when lower
    [InlineData(null, "aac", 2, 2, 256000)] // 128k per output channel
    [InlineData(null, "aac", 6, 2, 256000)]
    [InlineData(null, "aac", 2, null, 256000)] // falls back to input channels
    [InlineData(null, "aac", null, null, 384000)] // unknown channel layout
    [InlineData(null, "dts", 6, null, 768000)]
    [InlineData(null, "dca", 2, 2, 272000)] // 136k per channel for dts
    [InlineData(null, "flac", 2, null, 256000)] // default: 128k per channel
    [InlineData(null, "flac", null, null, 256000)] // default: assume stereo
    public void GetAudioBitrateParam_UsesCodecAndChannelLimits(int? requestedBitrate, string codec, int? inputChannels, int? outputChannels, int expected)
    {
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = inputChannels };

        Assert.Equal(expected, CreateHelper().GetAudioBitrateParam(requestedBitrate, codec, audioStream, outputChannels));
    }

    [Theory]
    [InlineData("libfdk_aac", 96000, 2, " -vbr:a 3")]
    [InlineData("libfdk_aac", 256000, 2, " -vbr:a 5")]
    [InlineData("libmp3lame", 192000, 2, " -qscale:a 2")]
    [InlineData("libmp3lame", 64000, 2, " -abr:a 1 -b:a 64000")] // out of lame's VBR sweet spot
    [InlineData("aac_at", 128000, 2, " -aac_at_mode:a 2 -b:a 128000")]
    [InlineData("libvorbis", 128000, 2, " -qscale:a 4")]
    [InlineData("aac", 128000, 2, null)]
    public void GetAudioVbrModeParam_MapsBitratePerChannelToQuality(string encoder, int bitrate, int channels, string? expected)
    {
        Assert.Equal(expected, CreateHelper().GetAudioVbrModeParam(encoder, bitrate, channels));
    }

    [Fact]
    public void GetVideoBitrateParamValue_NoVideoStream_UsesRequestedBitrate()
    {
        var request = new BaseEncodingJobOptions { VideoBitRate = 5_000_000 };

        Assert.Equal(5_000_000, CreateHelper().GetVideoBitrateParamValue(request, null!, "h264"));
    }

    [Fact]
    public void GetVideoBitrateParamValue_DoesNotExceedSourceBitrate()
    {
        var request = new BaseEncodingJobOptions { VideoBitRate = 10_000_000 };
        var videoStream = new MediaStream { Type = MediaStreamType.Video, Codec = "h264", BitRate = 4_000_000, Width = 1920, Height = 1080 };

        Assert.Equal(4_000_000, CreateHelper().GetVideoBitrateParamValue(request, videoStream, "h264"));
    }

    [Fact]
    public void GetVideoBitrateParamValue_UpscalingAllowsBitrateIncrease()
    {
        var request = new BaseEncodingJobOptions { VideoBitRate = 10_000_000, Width = 3840, Height = 2160 };
        var videoStream = new MediaStream { Type = MediaStreamType.Video, Codec = "h264", BitRate = 4_000_000, Width = 1920, Height = 1080 };

        Assert.Equal(10_000_000, CreateHelper().GetVideoBitrateParamValue(request, videoStream, "h264"));
    }

    [Fact]
    public void GetVideoBitrateParamValue_ScaledBitrateCappedByRequest()
    {
        var request = new BaseEncodingJobOptions { VideoBitRate = 10_000_000 };
        var videoStream = new MediaStream { Type = MediaStreamType.Video, Codec = "hevc", BitRate = 20_000_000, Width = 1920, Height = 1080 };

        Assert.Equal(10_000_000, CreateHelper().GetVideoBitrateParamValue(request, videoStream, "h264"));
    }

    [Fact]
    public void GetVideoBitrateParamValue_CappedAt400Mbps()
    {
        var request = new BaseEncodingJobOptions { VideoBitRate = 500_000_000 };

        Assert.Equal(400_000_000, CreateHelper().GetVideoBitrateParamValue(request, null!, "h264"));
    }

    [Theory]
    [InlineData("h264", "High", 4)]
    [InlineData("h264", "high 10", 7)] // spaces stripped, case-insensitive
    [InlineData("h264", "ConstrainedBaseline", 0)]
    [InlineData("hevc", "Main 10", 1)]
    [InlineData("av1", "Professional", 2)]
    [InlineData("h264", "unknown", -1)]
    [InlineData("vp9", "Main", -1)] // unscored codec
    public void GetVideoProfileScore_RanksProfiles(string codec, string profile, int expected)
    {
        Assert.Equal(expected, CreateHelper().GetVideoProfileScore(codec, profile));
    }

    [Fact]
    public void GetMediaStream_DesiredIndex_ReturnsMatchingStream()
    {
        var streams = BuildAudioStreams();
        var result = CreateHelper().GetMediaStream(streams, 2, MediaStreamType.Audio);

        Assert.NotNull(result);
        Assert.Equal(2, result.Index);
    }

    [Fact]
    public void GetMediaStream_NoIndex_PrefersAudioStreamWithChannels()
    {
        var streams = BuildAudioStreams();
        var result = CreateHelper().GetMediaStream(streams, null, MediaStreamType.Audio);

        Assert.NotNull(result);
        Assert.Equal(2, result.Index); // first audio stream that reports channels
    }

    [Fact]
    public void GetMediaStream_UnmatchedIndexWithoutFallback_ReturnsNull()
    {
        var streams = BuildAudioStreams();

        Assert.Null(CreateHelper().GetMediaStream(streams, 9, MediaStreamType.Audio, returnFirstIfNoIndex: false));
    }

    [Fact]
    public void GetFixedOutputSize_ScalesDownPreservingAspectRatio()
    {
        var (width, height) = EncodingHelper.GetFixedOutputSize(1920, 1080, null, null, 1280, 720);

        Assert.Equal(1280, width);
        Assert.Equal(720, height);
    }

    [Fact]
    public void GetFixedOutputSize_CapsAt4k()
    {
        var (width, height) = EncodingHelper.GetFixedOutputSize(7680, 4320, null, null, null, null);

        Assert.Equal(4096, width);
        Assert.Equal(2304, height);
    }

    [Fact]
    public void GetFixedOutputSize_RoundsToEvenDimensions()
    {
        var (width, height) = EncodingHelper.GetFixedOutputSize(1919, 1079, null, null, null, null);

        Assert.Equal(1918, width);
        Assert.Equal(1078, height);
    }

    [Fact]
    public void GetFixedOutputSize_MissingDimensions_ReturnsNull()
    {
        var (width, height) = EncodingHelper.GetFixedOutputSize(null, 1080, null, null, 1280, 720);

        Assert.Null(width);
        Assert.Null(height);
    }

    [Fact]
    public void EnforceResolutionLimit_ConvertsFixedSizeToCeiling()
    {
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            BaseRequest = new BaseEncodingJobOptions { Width = 1920, Height = 1080 }
        };

        CreateHelper().EnforceResolutionLimit(state);

        Assert.Equal(1920, state.BaseRequest.MaxWidth);
        Assert.Equal(1080, state.BaseRequest.MaxHeight);
        Assert.Null(state.BaseRequest.Width);
        Assert.Null(state.BaseRequest.Height);
    }

    [Fact]
    public void EnforceResolutionLimit_ExistingCeilingIsKept()
    {
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            BaseRequest = new BaseEncodingJobOptions { Width = 1920, Height = 1080, MaxWidth = 1280, MaxHeight = 720 }
        };

        CreateHelper().EnforceResolutionLimit(state);

        Assert.Equal(1280, state.BaseRequest.MaxWidth);
        Assert.Equal(720, state.BaseRequest.MaxHeight);
    }

    [Theory]
    [InlineData("mp3", "libmp3lame")]
    [InlineData("vorbis", "libvorbis")]
    [InlineData("opus", "libopus")]
    [InlineData("flac", "flac")]
    [InlineData("dts", "dca")]
    [InlineData("alac", "alac")]
    [InlineData("EAC3", "eac3")]
    [InlineData("invalid codec!", "aac")] // invalid codec names fall back to aac
    public void GetAudioEncoder_MapsCodecToEncoder(string outputAudioCodec, string expected)
    {
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            OutputAudioCodec = outputAudioCodec,
            BaseRequest = new BaseEncodingJobOptions()
        };

        Assert.Equal(expected, CreateHelper().GetAudioEncoder(state));
    }

    [Theory]
    [InlineData("aac_at", "aac_at")]
    [InlineData("libfdk_aac", "libfdk_aac")]
    [InlineData(null, "aac")]
    public void GetAudioEncoder_Aac_PrefersHigherQualityEncoders(string? supportedEncoder, string expected)
    {
        var state = new EncodingJobInfo(TranscodingJobType.Progressive)
        {
            OutputAudioCodec = "aac",
            BaseRequest = new BaseEncodingJobOptions()
        };

        var helper = CreateHelper(mediaEncoder =>
            mediaEncoder
                .Setup(e => e.SupportsEncoder(It.IsAny<string>()))
                .Returns((string name) => string.Equals(name, supportedEncoder, StringComparison.Ordinal)));

        Assert.Equal(expected, helper.GetAudioEncoder(state));
    }

    [Fact]
    public void GetNumAudioChannelsParam_NullAudioStream_ReturnsNull()
    {
        var state = BuildAudioChannelState("aac");

        Assert.Null(CreateHelper().GetNumAudioChannelsParam(state, null!, "aac"));
    }

    [Fact]
    public void GetNumAudioChannelsParam_CopyCodec_PassesThroughInputChannels()
    {
        var state = BuildAudioChannelState("copy");
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = 6 };

        Assert.Equal(6, CreateHelper().GetNumAudioChannelsParam(state, audioStream, "copy"));
    }

    [Fact]
    public void GetNumAudioChannelsParam_TranscodingMaxAudioChannels_LimitsOutput()
    {
        var state = BuildAudioChannelState("aac", transcodingMaxAudioChannels: 2);
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = 6 };

        Assert.Equal(2, CreateHelper().GetNumAudioChannelsParam(state, audioStream, "aac"));
    }

    [Fact]
    public void GetNumAudioChannelsParam_Mp3_LimitedToStereo()
    {
        var state = BuildAudioChannelState("mp3");
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = 6 };

        Assert.Equal(2, CreateHelper().GetNumAudioChannelsParam(state, audioStream, "mp3"));
    }

    [Fact]
    public void GetNumAudioChannelsParam_HlsWithUncommonLayout_DownmixesToStereo()
    {
        var state = BuildAudioChannelState("aac", TranscodingJobType.Hls);
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = 3 };

        Assert.Equal(2, CreateHelper().GetNumAudioChannelsParam(state, audioStream, "aac"));
    }

    [Fact]
    public void GetNumAudioChannelsParam_HlsWith5Channels_AddsLfeFor51Layout()
    {
        var state = BuildAudioChannelState("aac", TranscodingJobType.Hls);
        var audioStream = new MediaStream { Type = MediaStreamType.Audio, Channels = 5 };

        Assert.Equal(6, CreateHelper().GetNumAudioChannelsParam(state, audioStream, "aac"));
    }

    private static EncodingJobInfo BuildAudioChannelState(
        string outputAudioCodec,
        TranscodingJobType jobType = TranscodingJobType.Progressive,
        int? transcodingMaxAudioChannels = null)
    {
        return new EncodingJobInfo(jobType)
        {
            OutputAudioCodec = outputAudioCodec,
            BaseRequest = new BaseEncodingJobOptions
            {
                TranscodingMaxAudioChannels = transcodingMaxAudioChannels
            }
        };
    }

    private static List<MediaStream> BuildAudioStreams()
    {
        return
        [
            new MediaStream { Index = 0, Type = MediaStreamType.Video, Codec = "h264" },
            new MediaStream { Index = 1, Type = MediaStreamType.Audio, Codec = "truehd" },
            new MediaStream { Index = 2, Type = MediaStreamType.Audio, Codec = "ac3", Channels = 6 },
            new MediaStream { Index = 3, Type = MediaStreamType.Audio, Codec = "aac", Channels = 2 }
        ];
    }

    private static EncodingHelper CreateHelper(Action<Mock<IMediaEncoder>>? configureEncoder = null)
    {
        var mediaEncoder = new Mock<IMediaEncoder>();
        configureEncoder?.Invoke(mediaEncoder);

        return new EncodingHelper(
            Mock.Of<IApplicationPaths>(),
            mediaEncoder.Object,
            Mock.Of<ISubtitleEncoder>(),
            Mock.Of<IConfiguration>(),
            Mock.Of<IConfigurationManager>(),
            Mock.Of<IPathManager>());
    }
}
