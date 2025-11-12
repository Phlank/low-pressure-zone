namespace LowPressureZone.Api.Constants;

public class AudioCodecs
{
    public static readonly IEnumerable<string> Lossless = ["flac", "pcm_s16le", "pcm_s16be", "pcm_s24le", "pcm_s24be"];
    public static readonly IEnumerable<string> Lossy = ["mp3", "aac", "opus", "vorbis"];
    public static readonly IEnumerable<string> Allowed = Lossless.Union(Lossy);
    public static readonly IEnumerable<string> AllowedCodecFormats = ["flac", "wav", "aiff", "mp3", "m4a", "ogg/vorbis", "ogg/opus"];
}