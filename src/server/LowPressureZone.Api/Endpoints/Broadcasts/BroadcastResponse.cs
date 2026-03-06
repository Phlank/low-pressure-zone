namespace LowPressureZone.Api.Endpoints.Broadcasts;

public sealed class BroadcastResponse
{
    public int BroadcastId { get; set; }
    public int? StreamerId { get; set; }
    public string? StreamerDisplayName { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public bool IsDownloadable { get; set; }
    public bool IsArchivable { get; set; }
    public bool IsDisconnectable { get; set; }
}