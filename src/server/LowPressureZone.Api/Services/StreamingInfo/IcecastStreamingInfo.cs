namespace LowPressureZone.Api.Services.StreamingInfo;

public sealed class IcecastStreamingInfo
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
}