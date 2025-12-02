namespace LowPressureZone.Api.Constants;

public class MimeTypes
{
    private const string Flac = "audio/flac";
    private const string Mp3 = "audio/mpeg";
    private static readonly string[] Mp4 = ["audio/mp4", "audio/m4a", "audio/x-mp4", "audio/x-m4a"];
    private static readonly string[] Ogg = ["audio/ogg", "audio/x-ogg"];
    private static readonly string[] Wav = ["audio/wav", "audio/x-wav", "audio/wave", "audio/x-wave", "audio/vnd.wave", "audio/x-pn-wav", "audio/x-pn-wave"];
    
    public static IEnumerable<string> AudioMimeTypes => [Flac, ..Mp4, Mp3, ..Ogg, ..Wav];
}
