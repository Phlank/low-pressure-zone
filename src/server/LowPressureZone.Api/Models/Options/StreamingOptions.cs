using LowPressureZone.Adapter.AzuraCast.Options;
using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Models.Options;

public sealed class StreamingOptions
{
    public const string Name = "Streaming";
    public required StreamConnection Live { get; set; }
    public required StreamConnection Test { get; set; }
}

public sealed class StreamConnection
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
    public StreamCredentials? Credentials { get; set; }
}

public sealed class StreamCredentials
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}