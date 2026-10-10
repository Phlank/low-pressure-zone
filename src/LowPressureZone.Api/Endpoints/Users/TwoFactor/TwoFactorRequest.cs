namespace LowPressureZone.Api.Endpoints.Users.TwoFactor;

public class TwoFactorRequest
{
    public required string Code { get; set; }
    public bool RememberClient { get; set; }
}