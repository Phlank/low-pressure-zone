namespace LowPressureZone.Api.Services.StreamConnectionInfo;

public sealed class AzuraCastStreamingInfo
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
}