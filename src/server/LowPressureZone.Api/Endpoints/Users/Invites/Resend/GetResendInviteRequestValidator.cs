using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;

namespace LowPressureZone.Api.Endpoints.Users.Invites.Resend;

public class GetResendInviteRequestValidator : Validator<GetResendInviteRequest>
{
    public GetResendInviteRequestValidator()
    {
        RuleFor(r => r.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
    }
}