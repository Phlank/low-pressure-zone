using System.Linq;
using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Context).NotEmpty();
        RuleFor(r => r.Username).Username();
        RuleFor(r => r.Password).Password();
        RuleFor(r => r.ConfirmPassword).Must((request, confirmPassword) => request.Password == confirmPassword).WithMessage("Does not match");
    }
}
