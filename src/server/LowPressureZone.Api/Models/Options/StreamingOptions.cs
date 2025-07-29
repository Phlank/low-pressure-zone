using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Models.Options;

public sealed class StreamingOptions
{
    public const string Name = "Streaming";
    public required StreamUseType Primary { get; set; }
    public required IReadOnlyCollection<StreamInstanceOptions> Streams { get; set; }
}

public sealed class StreamInstanceOptions
{
    public required StreamUseType Use { get; init; }
    public required StreamServerType Server { get; init; }
    public required StreamConnectionOptions Connection { get; init; }
    public StreamCredentialOptions? Credentials { get; init; }
    public IcecastOptions? Icecast { get; init; }
    public AzuraCastOptions? AzuraCast { get; init; }
}

public sealed class StreamConnectionOptions
{
    public required string Host { get; init; }
    public required string Port { get; init; }
    public required string Mount { get; init; }
}

public sealed class StreamCredentialOptions
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}
