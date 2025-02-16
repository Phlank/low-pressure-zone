namespace LowPressureZone.Api.Endpoints.Users.Login;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string? TwoFactorCode { get; set; }
}