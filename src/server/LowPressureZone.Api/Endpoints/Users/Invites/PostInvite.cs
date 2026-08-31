using FastEndpoints;
using FluentEmail.Core;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Invites;

public class PostInvite(
    UserManager<AppUser> userManager,
    IdentityContext identityContext,
    DataContext dataContext,
    EmailService emailService)
    : EndpointWithMapper<InviteRequest, InviteMapper>
{
    public override void Configure()
    {
        Post("/users/invites");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(InviteRequest request, CancellationToken ct)
    {
        var invitation = Map.ToEntity(request);
        
        var community = await dataContext.Communities.FirstOrDefaultAsync(c => c.Id == request.CommunityId, ct);
        if (community is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var normalizedEmail = request.Email.ToUpperInvariant().Normalize();
        var username = Guid.NewGuid().ToString();
        var normalizedUsername = username.ToUpperInvariant().Normalize();
        var user = new AppUser
        {
            Id = invitation.UserId,
            Email = request.Email,
            NormalizedEmail = normalizedEmail,
            DisplayName = username,
            UserName = username,
            NormalizedUserName = normalizedUsername,
            LockoutEnabled = false,
            LockoutEnd = DateTimeOffset.MaxValue.ToUniversalTime()
        };
        var createResult = await userManager.CreateAsync(user);
        createResult.Errors.ForEach(e => AddError(e.Code + " " + e.Description));
        ThrowIfAnyErrors();

        await userManager.SendWelcomeEmail(user, emailService);

        identityContext.Add(invitation);
        await identityContext.SaveChangesAsync(ct);
        
        community.SetRolesForUser(user.Id, request.IsPerformer, request.IsOrganizer);
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}