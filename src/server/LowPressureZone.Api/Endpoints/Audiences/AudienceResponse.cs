namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public bool CanDelete { get; set; }
}
