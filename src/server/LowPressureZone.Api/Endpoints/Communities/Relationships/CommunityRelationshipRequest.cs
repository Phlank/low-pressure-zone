namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class CommunityRelationshipRequest
{
    public required bool IsPerformer { get; set; }
    public required bool IsOrganizer { get; set; }
}
