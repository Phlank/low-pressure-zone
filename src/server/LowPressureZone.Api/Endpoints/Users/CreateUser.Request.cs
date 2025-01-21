using FastEndpoints;
using static LowPressureZone.Api.Endpoints.Users.CreateUser;

namespace LowPressureZone.Api.Endpoints.Users;

public sealed class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
