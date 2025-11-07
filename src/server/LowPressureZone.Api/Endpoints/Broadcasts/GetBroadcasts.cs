using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class GetBroadcasts(UserManager<AppUser> userManager, AzuraCastClient client)
    : EndpointWithoutRequest<IEnumerable<BroadcastResponse>, BroadcastMapper>
{
    public override void Configure() => Get("/broadcasts");

    public override async Task HandleAsync(CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user?.StreamerId is null)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var broadcastsResult = await client.GetBroadcastsAsync();

        if (!broadcastsResult.IsSuccess)
            ThrowError(broadcastsResult.Error.ReasonPhrase ?? "Unknown reason",
                       (int)broadcastsResult.Error.StatusCode);

        IEnumerable<StationStreamerBroadcast> broadcasts = broadcastsResult.Value;
        if (!(User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)))
            broadcasts = broadcasts.Where(broadcast => broadcast.Streamer?.Id == user.StreamerId);

        var responses = broadcasts.OrderByDescending(broadcast => broadcast.TimestampStart)
                                  .Select(Map.FromEntity);

        await SendOkAsync(responses, ct);
    }
}
