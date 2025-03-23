namespace LowPressureZone.Api.Endpoints.Users.Login;

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
