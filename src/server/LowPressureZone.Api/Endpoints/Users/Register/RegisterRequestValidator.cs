using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Context).NotEmpty();
        RuleFor(request => request.DisplayName).NotEmpty()
                                               .WithMessage(Errors.Required)
                                               .MaximumLength(AppUser.DisplayNameMaxLength)
                                               .WithMessage(Errors.MaxLength(AppUser.DisplayNameMaxLength))
                                               .NotEqual(request => request.Username)
                                               .WithMessage(Errors.NotEqual(nameof(RegisterRequest.Username)));
        RuleFor(r => r.Username).Username();
        RuleFor(r => r.Password).Password();
        RuleFor(r => r.ConfirmPassword).Must((request, confirmPassword) => request.Password == confirmPassword)
                                       .WithMessage("Does not match");
    }
}
