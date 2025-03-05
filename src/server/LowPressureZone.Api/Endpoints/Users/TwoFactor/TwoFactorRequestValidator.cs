using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Users.TwoFactor;

public class TwoFactorRequestValidator : Validator<TwoFactorRequest>
{
    public TwoFactorRequestValidator()
    {
        RuleFor(r => r.Code).NotEmpty().WithMessage(Errors.Required);
    }
}
