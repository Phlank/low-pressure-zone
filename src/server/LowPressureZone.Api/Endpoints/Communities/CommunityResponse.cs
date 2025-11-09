namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool IsPerformable { get; set; }
    public required bool IsOrganizable { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
}