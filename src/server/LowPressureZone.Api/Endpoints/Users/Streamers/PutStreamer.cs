using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers;

public class PutStreamer(UserManager<AppUser> userManager, AzuraCastClient client) : Endpoint<StreamerRequest>
{
    public override void Configure() => Put("/users/streamers");

    public override async Task HandleAsync(StreamerRequest req, CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user?.StreamerId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var streamerResult = await userManager.GetStreamerAsync(user, client);
        if (!streamerResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var streamerRequest = streamerResult.Value;
        streamerRequest.DisplayName = req.DisplayName;
        var updateResult = await client.UpdateStreamerAsync(streamerRequest);
        if (!updateResult.IsSuccess)
        {
            AddError("Could not update broadcast information");
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}
