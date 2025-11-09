namespace LowPressureZone.Api.Endpoints.Users.ResetPassword;

public class PostResetPasswordRequest
{
    public required string Context { get; set; }
    public required string Password { get; set; }
}