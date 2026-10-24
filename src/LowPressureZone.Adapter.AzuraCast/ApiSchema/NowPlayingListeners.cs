namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public record NowPlayingListeners
{
    public int Total { get; set; }
    public int Unique { get; set; }
    public int Current { get; set; }
}