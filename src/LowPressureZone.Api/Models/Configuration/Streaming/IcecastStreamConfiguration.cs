namespace LowPressureZone.Api.Models.Configuration.Streaming;

public sealed class IcecastStreamConfiguration
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}