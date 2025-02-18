namespace LowPressureZone.Api.Endpoints.Users.Info;

public class UserInfoResponse
{
    public required string Username { get; set; }
    public required List<string> Roles { get; set; }
}
