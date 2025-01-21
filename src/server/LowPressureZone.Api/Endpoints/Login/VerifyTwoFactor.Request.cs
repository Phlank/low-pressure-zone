namespace LowPressureZone.Api.Endpoints.Login;

public class VerifyTwoFactorRequest
{
    public string Username { get; set; } = string.Empty;
    public string TwoFactorCode { get; set; } = string.Empty;
}