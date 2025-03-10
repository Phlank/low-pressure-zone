using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Invite;

public class InviteRequestValidator : Validator<InviteRequest>
{
    public InviteRequestValidator()
    {
        RuleFor(i => i.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
        RuleFor(i => i.Role).NotEmpty().WithMessage(Errors.Required).Must(r => RoleNames.All.Contains(r)).WithMessage(Errors.InvalidRole);

        RuleFor(req => req).CustomAsync(async (req, ctx, ct) =>
        {
            var identityContext = Resolve<IdentityContext>();
            var normalizedEmail = req.Email.ToUpperInvariant();

            var isEmailInUse = await identityContext.Users.Where(u => u.NormalizedEmail == normalizedEmail)
                                                          .AnyAsync(ct);
            if (isEmailInUse)
                ctx.AddFailure(nameof(req.Email), Errors.Unique);
        });
    }
}
