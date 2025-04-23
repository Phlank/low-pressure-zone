namespace LowPressureZone.Api.Endpoints.Status.Stream;

public class StreamStatusResponse
{
    public required bool IsOnline { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ListenUrl { get; set; }
}
