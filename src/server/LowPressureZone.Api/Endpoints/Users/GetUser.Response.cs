namespace LowPressureZone.Api.Endpoints.Users;

public class GetUserResponse
{
    public string Id { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}