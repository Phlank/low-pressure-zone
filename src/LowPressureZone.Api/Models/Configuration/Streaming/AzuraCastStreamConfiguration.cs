namespace LowPressureZone.Api.Models.Configuration.Streaming;

public sealed class AzuraCastStreamConfiguration
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
}