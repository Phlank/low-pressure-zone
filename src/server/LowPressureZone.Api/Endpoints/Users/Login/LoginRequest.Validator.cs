using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Users.Login;

public class LoginRequestValidator : Validator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(l => l.Username).NotEmpty();
        RuleFor(l => l.Password).NotEmpty();
    }
}
