namespace LowPressureZone.Api.Endpoints.Audience;

public sealed class AudienceRequest
{
    public required string Name { get; set; }
    public required string Url { get; set; }
}
