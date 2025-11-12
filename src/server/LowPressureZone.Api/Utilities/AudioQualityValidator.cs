using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Utilities;

public static class AudioQualityValidator
{
    public static IEnumerable<ValidationFailure> ValidateAudioQuality(IMediaAnalysis analysis, string? propertyName = null)
    {
        ICollection<ValidationFailure> failures = [];
        if (analysis.AudioStreams.Count == 0)
            failures.Add(new(propertyName,
                             "One audio stream must be present in the media file."));
        else if (analysis.AudioStreams.Count > 1)
            failures.Add(new(propertyName,
                             "Only one audio stream should be present in the media file."));

        if (failures.Count > 0)
            return failures;
        
        var stream = analysis.AudioStreams[0];
        if (!AudioCodecs.Allowed.Contains(stream.CodecName))
            failures.Add(new(propertyName,
                             $"Audio codec '{stream.CodecName}' is not supported. Supported codecs are: {string.Join(", ", AudioCodecs.AllowedCodecFormats)}."));
        if (failures.Count > 0)
            return failures;
        
        if (stream.Channels != 2)
            failures.Add(new(propertyName,
                             "Audio stream must be stereo (2 channels)."));

        if (stream.SampleRateHz < 44100)
            failures.Add(new(propertyName,
                             "Audio sample rate must be at least 44.1 kHz."));

        if (AudioCodecs.Lossy.Contains(stream.CodecName))
        {
            if (stream.BitRate < 256000)
                failures.Add(new(propertyName,
                                 $"Audio bitrate must be at least 256 kbps when using {stream.CodecName}."));
        }

        return failures;
    }
}