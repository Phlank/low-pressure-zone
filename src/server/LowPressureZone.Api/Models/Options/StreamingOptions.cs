namespace LowPressureZone.Api.Models.Options;

public sealed class StreamingOptions
{
    public const string Name = "Streaming";
    public required string ServerType { get; set; }
    public required StreamInstanceConfiguration LiveInfo { get; set; }
    public required StreamInstanceConfiguration TestInfo { get; set; }

    public sealed class StreamInstanceConfiguration
    {
        public required StreamConnectionConfiguration Connection { get; set; }
        public StreamUserConfiguration? User { get; set; }
    }

    public sealed class StreamConnectionConfiguration
    {
        public required string Host { get; set; }
        public required string Port { get; set; }
        public required string Mount { get; set; }
        public required string Type { get; set; }
    }

    public sealed class StreamUserConfiguration
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? DisplayName { get; set; }
    }
}
