namespace LowPressureZone.Api.Endpoints.Stream.ConnectionInfo;

public class ConnectionInfoResponse
{
    public required string StreamType { get; set; }
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Type { get; set; }
    public required string StreamTitleField { get; set; }
}
