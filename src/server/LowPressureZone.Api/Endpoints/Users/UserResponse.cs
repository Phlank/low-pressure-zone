
namespace LowPressureZone.Api.Endpoints.Users;

public class UserResponse
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required IEnumerable<string> Roles { get; set; } = [];
    public required DateTime? RegistrationDate { get; set; }
}
