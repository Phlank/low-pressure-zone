namespace LowPressureZone.Api.Models.Stream.AzuraCast.Schema;

public record Listeners
{
    public int Total { get; set; }
    public int Unique { get; set; }
    public int Current { get; set; }
}
