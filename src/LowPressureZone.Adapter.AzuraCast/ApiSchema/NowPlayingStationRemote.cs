namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingStationRemote
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public int? Bitrate { get; set; }
    public string? Format { get; set; }
    public required NowPlayingListeners Listeners { get; set; }
}