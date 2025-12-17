namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityRequest
{
    public required string Name { get; set; }
    public required string Url { get; set; }
}