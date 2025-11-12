namespace LowPressureZone.Api.Constants;

public class AudioCodecs
{
    public static readonly IEnumerable<string> Lossless = ["flac", "wav", "aiff"];
    public static readonly IEnumerable<string> Lossy = ["mp3", "aac", "ogg", "opus"];
    public static readonly IEnumerable<string> Allowed = Lossless.Union(Lossy);
}