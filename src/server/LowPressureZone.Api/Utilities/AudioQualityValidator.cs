using FFMpegCore;
using FluentValidation.Results;

namespace LowPressureZone.Api.Utilities;

public static class AudioQualityValidator
{
    public static IEnumerable<ValidationFailure> ValidateAudioQuality(IMediaAnalysis analysis, long fileSize, string? propertyName = null)
    {
        List<ValidationFailure> failures = [];
        if (analysis.AudioStreams.Count == 0)
            failures.Add(new ValidationFailure(propertyName,
                                               "No audio stream found in file"));
        else if (analysis.AudioStreams.Count > 1)
            failures.Add(new ValidationFailure(propertyName,
                                               "Multiple audio streams found in file"));

        if (failures.Count > 0)
            return failures;
        
        var stream = analysis.AudioStreams[0];
        
        if (stream.Channels != 2)
            failures.Add(new ValidationFailure(propertyName,
                                               "Audio file must be stereo"));

        if (stream.SampleRateHz < 44100)
            failures.Add(new ValidationFailure(propertyName,
                                               "Minimum sample rate is 44.1 kHz"));
        
        failures.AddRange(ValidateCodec(stream, fileSize, propertyName));

        return failures;
    }

    private static List<ValidationFailure> ValidateCodec(AudioStream stream, long fileSize, string? propertyName = null)
    {
        List<ValidationFailure> failures = [];

        if (!stream.CodecName.StartsWith("pcm_", StringComparison.InvariantCulture)
            && stream.CodecName != "flac"
            && stream.CodecName != "mpeg"
            && stream.CodecName != "mp3"
            && stream.CodecName != "aac"
            && stream.CodecName != "vorbis"
            && stream.CodecName != "opus")
        {
            failures.Add(new ValidationFailure(propertyName, "Unsupported audio codec"));
        }
        
        if (LossyCodecs.Contains(stream.CodecName))
        {
            long bitrate;
            if (stream.BitRate == 0)
                bitrate = (fileSize / (long)stream.Duration.TotalSeconds) * 8;
            else
                bitrate = stream.BitRate;
            
            
            if (bitrate < 256000)
                failures.Add(new ValidationFailure(propertyName, "Minimum bitrate is 256 kbps"));
        }
        
        return failures;
    }

    private static readonly HashSet<string> LossyCodecs = ["mpeg", "aac", "vorbis", "opus"];
}