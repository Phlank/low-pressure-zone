using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Users.ResetPassword;

public class PostResetPasswordRequestValidator : Validator<PostResetPasswordRequest>
{
    public PostResetPasswordRequestValidator()
    {
        RuleFor(r => r.Context).NotEmpty().WithMessage(Errors.Required);
        RuleFor(r => r.Password).NotEmpty().WithMessage(Errors.Required).Password();
    }
}
