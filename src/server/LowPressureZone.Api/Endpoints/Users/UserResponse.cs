﻿namespace LowPressureZone.Api.Endpoints.Users;

public class UserResponse
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required List<string> Roles { get; set; } = new List<string>();
}
