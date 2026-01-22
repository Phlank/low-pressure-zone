using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers;

public class PutStreamer(UserManager<AppUser> userManager, IAzuraCastClient client) : Endpoint<StreamerRequest>
{
    public override void Configure() => Put("/users/streamers");

    public override async Task HandleAsync(StreamerRequest req, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user?.StreamerId is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var streamerResult = await userManager.GetStreamerAsync(user, client);
        if (!streamerResult.IsSuccess)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var streamerRequest = streamerResult.Value;
        streamerRequest.DisplayName = req.DisplayName;
        var updateResult = await client.PutStreamerAsync(streamerRequest);
        if (!updateResult.IsSuccess) ThrowError("Could not update broadcast information");

        await Send.NoContentAsync(ct);
    }
}