namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public class Broadcast
{
    public int Id { get; set; }
    public DateTime TimestampStart { get; set; }
    public DateTime? TimestampEnd { get; set; }
    public Streamer? Streamer { get; set; }
    public BroadcastRecording? Recording { get; set; }
}
