namespace LowPressureZone.Api.Models.Options;

public class AzuraCastOptions
{
    public const string Name = "AzuraCast";

    public required Uri ApiUrl { get; init; }
    public required string ApiKey { get; init; }
    public required string StationId { get; init; }
    public required string SftpUrl { get; set; }
    public required int SftpPort { get; set; }
    public required string SftpUsername { get; set; }
    public required string SftpPassword { get; set; }
    public required string MediaDirectory { get; set; }
}
