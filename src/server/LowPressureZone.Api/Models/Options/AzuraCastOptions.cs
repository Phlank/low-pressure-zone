namespace LowPressureZone.Api.Models.Options;

public class AzuraCastOptions
{
    public const string Name = "AzuraCast";

    public required Uri ApiUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string StationId { get; set; }
}
