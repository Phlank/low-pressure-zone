using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Data;
using LowPressureZone.Domain.BroadcastAggregate;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class GetBroadcasts(UserManager<AppUser> userManager, DataContext dataContext, IAzuraCastClient client)
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

        List<Broadcast> broadcasts;
        if (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer))
        {
            broadcasts = await dataContext.Broadcasts.ToListAsync(ct);
        }
        else
        {
            broadcasts = await dataContext.Broadcasts
                                          .Where(b => b.AzuraCastStreamerId == user.StreamerId)
                                          .ToListAsync(ct);
        }
        
        await Send.OkAsync(broadcasts.Select(Map.FromEntity), ct);
    }
}