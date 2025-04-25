namespace LowPressureZone.Api.Endpoints.Icecast.Status;

public class IcecastStatusResponse
{
    public required bool IsOnline { get; set; }
    public required bool IsLive { get; set; }
    public required bool IsStatusStale { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ListenUrl { get; set; }
}
