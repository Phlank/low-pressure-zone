namespace LowPressureZone.Api.Endpoints.Communities.Relationships;

public class CommunityRelationshipResponse
{
    public required Guid CommunityId { get; set; }
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
    public required bool IsPerformer { get; set; }
    public required bool IsOrganizer { get; set; }
}
