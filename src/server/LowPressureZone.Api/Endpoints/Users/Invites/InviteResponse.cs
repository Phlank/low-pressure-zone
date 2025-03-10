namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteResponse
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required DateTime InviteDate { get; set; }
}
