namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
    public required bool IsLinkableToSchedule { get; set; }
}
