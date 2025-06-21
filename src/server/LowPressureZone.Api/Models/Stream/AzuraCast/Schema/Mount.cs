namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public record Mount : Remote
{
    public string Path { get; set; } = string.Empty;
    public bool? IsDefault { get; set; }
}
