namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteRequest
{
    public required string Email { get; set; }
    public required Guid CommunityId { get; set; }
    public required bool IsPerformer { get; set; }
    public required bool IsOrganizer { get; set; }
}