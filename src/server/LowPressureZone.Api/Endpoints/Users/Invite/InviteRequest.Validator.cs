using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class InviteRequestValidator : Validator<InviteRequest>
{
    public InviteRequestValidator()
    {
        RuleFor(i => i.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
    }
}
