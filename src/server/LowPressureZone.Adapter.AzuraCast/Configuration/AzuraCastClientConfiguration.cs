namespace LowPressureZone.Adapter.AzuraCast.Configuration;

public sealed class AzuraCastClientConfiguration
{
    public const string Name = "AzuraCast";

    public required Uri ApiUrl { get; init; }
    public required string ApiKey { get; init; }
    public required string StationId { get; init; }
}