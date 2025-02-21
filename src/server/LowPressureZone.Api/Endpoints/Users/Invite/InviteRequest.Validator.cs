using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class InviteRequestValidator : Validator<InviteRequest>
{
    public InviteRequestValidator()
    {
        RuleFor(i => i.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
        RuleFor(i => i.Role).NotEmpty().WithMessage(Errors.Required).Must(r => RoleNames.All.Contains(r)).WithMessage(Errors.InvalidRole);
    }
}
