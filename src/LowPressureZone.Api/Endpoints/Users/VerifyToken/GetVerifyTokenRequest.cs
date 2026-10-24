namespace LowPressureZone.Api.Endpoints.Users.VerifyToken;

public class GetVerifyTokenRequest
{
    public required string Context { get; set; }
    public required string Purpose { get; set; }
    public required string Provider { get; set; }
}