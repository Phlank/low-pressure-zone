namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public class BroadcastRecording
{
    public string Path { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public int Size { get; set; }
}
