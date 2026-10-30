using FastEndpoints;
using LowPressureZone.Identity.Application.Commands.DisableStreamer;
using LowPressureZone.Identity.Application.Commands.EnableStreamer;
using LowPressureZone.Identity.Application.Commands.Lock;
using LowPressureZone.Identity.Application.Commands.Unlock;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Enabled;

public class PutEnabled(UserManager<AppUser> userManager) : Endpoint<PutEnabledRequest>
{
    public override void Configure()
    {
        Put("/users/{id}/enabled");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(PutEnabledRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        
        // Non-Admin users cannot change enabled status of Admins and Organizers
        var roles = await userManager.GetRolesAsync(user);
        if (!User.IsInRole(RoleNames.Admin) && (roles.Contains(RoleNames.Organizer) || roles.Contains(RoleNames.Admin)))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (req.Enabled)
        {
            await new EnableStreamerCommand(id).ExecuteAsync(ct);
            await new UnlockUserCommand(id).ExecuteAsync(ct);
        }
        else
        {
            await new DisableStreamerCommand(id).ExecuteAsync(ct);
            await new LockUserCommand(id).ExecuteAsync(ct);
        }
    }
}