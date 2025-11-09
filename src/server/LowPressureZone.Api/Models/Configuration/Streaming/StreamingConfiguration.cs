namespace LowPressureZone.Api.Models.Configuration.Streaming;

public sealed class StreamingConfiguration
{
    public const string Name = "Streaming";
    public required AzuraCastStreamConfiguration Live { get; set; }
    public required IcecastStreamConfiguration Test { get; set; }
}