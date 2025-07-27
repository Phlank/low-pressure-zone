using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Models.Options;

public sealed class StreamingOptions
{
    public const string Name = "Streaming";
    public required StreamInstanceOptions Live { get; init; }
    public required StreamInstanceOptions Test { get; init; }
}

public sealed class StreamInstanceOptions
{
    public required StreamConnectionOptions Connection { get; set; }
    public StreamUserOptions? User { get; set; }
}

public sealed class StreamConnectionOptions
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Mount { get; set; }
    public required StreamServerType Type { get; set; }
}

public sealed class StreamUserOptions
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? DisplayName { get; set; }
}
