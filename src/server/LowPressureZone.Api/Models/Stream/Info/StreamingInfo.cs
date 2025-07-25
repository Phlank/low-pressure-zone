namespace LowPressureZone.Api.Models.Stream.Info;

public class StreamingInfo
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public required string Mount { get; set; }
    public required string Type { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? DisplayName { get; set; }
}
