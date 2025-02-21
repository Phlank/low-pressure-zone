namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class InviteRequest
{
    public required string Email { get; set; }
    public required string Role { get; set; }
}