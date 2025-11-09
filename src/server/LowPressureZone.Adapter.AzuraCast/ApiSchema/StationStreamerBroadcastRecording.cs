namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationStreamerBroadcastRecording
{
    public string Path { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public int Size { get; set; }
}