using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;

namespace LowPressureZone.Api.Endpoints.Users.Register;

public class RegisterRequestValidator : Validator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Context).NotEmpty();
        RuleFor(request => request.DisplayName).NotEmpty()
                                               .WithMessage(Errors.Required)
                                               .MaximumLength(AppUser.DisplayNameMaxLength)
                                               .WithMessage(Errors.MaxLength(AppUser.DisplayNameMaxLength));
        RuleFor(request => request.Username).Username();
        RuleFor(request => request.Password).Password();
        RuleFor(request => request.ConfirmPassword)
            .Must((request, confirmPassword) => request.Password == confirmPassword)
            .WithMessage("Does not match");
    }
}