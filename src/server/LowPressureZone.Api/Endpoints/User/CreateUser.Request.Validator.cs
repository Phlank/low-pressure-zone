using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using static LowPressureZone.Api.Endpoints.User.CreateUser;

namespace LowPressureZone.Api.Endpoints.User;

public sealed class CreateUserRequestValidator : Validator<CreateUserRequest>
{
    private const string USERNAME_INVALID_CHARS_MESSAGE = "Username contains invalid characters. Use A-z, 0-9, ., _, and - only.";
    private static Regex usernameRegex = new Regex("^[A-z0-9._-]$");

    public CreateUserRequestValidator()
    {
        RuleFor(r => r.Username).NotEmpty().MaximumLength(32).Matches(usernameRegex).WithMessage(USERNAME_INVALID_CHARS_MESSAGE);
        RuleFor(r => r.Password).MinimumLength(10).MaximumLength(64);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Name).NotEmpty();
    }
}
