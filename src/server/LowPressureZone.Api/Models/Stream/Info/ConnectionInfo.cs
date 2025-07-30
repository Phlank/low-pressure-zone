namespace LowPressureZone.Api.Models.Stream.Info;

public class ConnectionInfo
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Mount { get; set; }
    public required string Type { get; set; }
}
