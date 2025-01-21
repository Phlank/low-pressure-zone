using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Users;

public sealed class CreateUserValidator : Validator<CreateUserRequest>
{
    private const string USERNAME_INVALID_CHARS_MESSAGE = "Username contains invalid characters. Use A-z, 0-9, ., _, and - only.";
    private static Regex usernameRegex = new Regex("[A-Za-z0-9._-]+");

    public CreateUserValidator()
    {
        RuleFor(r => r.Username).NotEmpty().MaximumLength(32).Matches(usernameRegex).WithMessage(USERNAME_INVALID_CHARS_MESSAGE);
        RuleFor(r => r.Password).MinimumLength(10).MaximumLength(64);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
    }
}
