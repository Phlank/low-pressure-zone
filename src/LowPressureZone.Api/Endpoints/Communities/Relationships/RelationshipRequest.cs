namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class RelationshipRequest
{
    public required bool IsPerformer { get; set; }
    public required bool IsOrganizer { get; set; }
}