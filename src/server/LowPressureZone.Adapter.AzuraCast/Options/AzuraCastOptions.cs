namespace LowPressureZone.Adapter.AzuraCast.Options;

public sealed class AzuraCastOptions
{
    public const string Name = "AzuraCast";

    public required Uri ApiUrl { get; init; }
    public required string ApiKey { get; init; }
    public required string StationId { get; init; }
}
