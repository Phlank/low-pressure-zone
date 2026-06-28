using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers;

public class PostDisable(UserManager<AppUser> userManager, IAzuraCastClient azuraCastClient) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/users/{id}/streamers/disable");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null || user.StreamerId is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }
        
        var updateStreamerResult = await azuraCastClient.DisableStreamerAsync(user.StreamerId.Value);
        if (updateStreamerResult.IsError)
        {
            throw new InvalidOperationException("Failed to update streamer");
        }

        await Send.OkAsync(null, ct);
    }
}