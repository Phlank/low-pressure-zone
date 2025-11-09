namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class StationStreamerBroadcast
{
    public int Id { get; set; }
    public DateTime TimestampStart { get; set; }
    public DateTime? TimestampEnd { get; set; }
    public StationStreamerBroadcastStreamer? Streamer { get; set; }
    public StationStreamerBroadcastRecording? Recording { get; set; }
}