using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class InviteRequestValidator : Validator<InviteRequest>
{
    public InviteRequestValidator()
    {
        RuleFor(i => i.Email).NotEmpty().WithMessage(Errors.Required).EmailAddress().WithMessage(Errors.InvalidEmail);
        RuleFor(i => i.CommunityId).NotEmpty().WithMessage(Errors.Required);

        RuleFor(req => req).CustomAsync(async (req, ctx, ct) =>
        {
            var dataContext = Resolve<DataContext>();
            var identityContext = Resolve<IdentityContext>();
            var normalizedEmail = req.Email.ToUpperInvariant().Normalize();

            var isEmailInUse = await identityContext.Users
                                                    .Where(u => u.NormalizedEmail == normalizedEmail)
                                                    .AnyAsync(ct);
            if (isEmailInUse)
                ctx.AddFailure(nameof(req.Email), Errors.Unique);
            
            if (!await dataContext.Communities
                                  .AsNoTracking()
                                  .Where(community => community.Id == req.CommunityId && !community.IsDeleted)
                                  .AnyAsync(ct))
                ctx.AddFailure(nameof(req.CommunityId), Errors.DoesNotExist);
        });
    }
}
