namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class BroadcastResponse
{
    public int? StreamerId { get; set; }
    public string? BroadcasterDisplayName { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public bool IsDownloadable { get; set; }
    public bool IsDeletable { get; set; }
    public string? RecordingPath { get; set; }
    public string? NearestPerformerName { get; set; }
    public int BroadcastId { get; set; }
}
