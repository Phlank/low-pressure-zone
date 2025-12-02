namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteResponse
{
    public required Guid Id { get; set; }
    public required Guid CommunityId { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required DateTime InvitedAt { get; set; }
    public required DateTime LastSentAt { get; set; }
}