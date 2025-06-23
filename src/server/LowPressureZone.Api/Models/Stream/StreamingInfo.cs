namespace LowPressureZone.Api.Models.Stream;

public class StreamingInfo
{
    public required string Host { get; set; }
    public required string Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public required string Type { get; set; }
    public required string StreamTitleField { get; set; }
}
