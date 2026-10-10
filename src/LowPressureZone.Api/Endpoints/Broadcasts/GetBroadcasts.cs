using FastEndpoints;
using LowPressureZone.Api.Commands.Broadcasts.SyncBroadcasts;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class GetBroadcasts(UserManager<AppUser> userManager)
    : EndpointWithoutRequest<IEnumerable<BroadcastResponse>, BroadcastMapper>
{
    public override void Configure() => Get("/broadcasts");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user?.StreamerId is null)
        {
            await Send.ForbiddenAsync(ct);
            return;
        }

        var broadcasts = await new SyncBroadcastsCommand().ExecuteAsync(ct);
        
        if (!User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.Organizer))
        {
            broadcasts = [ ..broadcasts.Where(b => b.AzuraCastStreamerId == user.StreamerId)];
        }
        
        await Send.OkAsync(broadcasts.Select(Map.FromEntity), ct);
    }
}