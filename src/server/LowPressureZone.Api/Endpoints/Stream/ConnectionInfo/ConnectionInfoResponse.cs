namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class ConnectionInfoResponse
{
    public required string StreamType { get; set; }
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Mount { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public required string Type { get; set; }
    public string? DisplayName { get; set; }
    public bool IsDisplayNameEditable => Type == "Live Stream";
}
