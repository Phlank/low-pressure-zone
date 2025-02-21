﻿namespace LowPressureZone.Api.Endpoints.Users.Register;

public class RegisterRequest
{
    public string? Context { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}