namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public record Remote
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public int? Bitrate { get; set; }
    public string? Format { get; set; }
    public required Listeners Listeners { get; set; }
}
