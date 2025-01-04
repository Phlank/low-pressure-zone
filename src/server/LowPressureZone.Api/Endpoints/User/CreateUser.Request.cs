using FastEndpoints;
using static LowPressureZone.Api.Endpoints.User.CreateUser;

namespace LowPressureZone.Api.Endpoints.User;

public sealed class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
