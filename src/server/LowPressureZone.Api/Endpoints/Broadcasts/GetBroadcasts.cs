using System.Collections.Immutable;
using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class GetBroadcasts(
    UserManager<AppUser> userManager,
    AzuraCastClient client,
    DataContext context) : EndpointWithoutRequest<IEnumerable<BroadcastResponse>, BroadcastMapper>
{
    private const int BroadcastBufferMinutes = 20;

    public override void Configure()
    {
        Get("/broadcasts");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

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

        var broadcasts = broadcastsResult.Value;
        if (!User.IsInRole(RoleNames.Admin))
            broadcasts = broadcasts.Where(broadcast => broadcast.Streamer?.Id == user.StreamerId).ToImmutableList();

        var orderedBroadcasts = broadcasts.OrderByDescending(broadcast => broadcast.TimestampStart);
        var responses = orderedBroadcasts.Select(Map.FromEntity);

        await SendOkAsync(responses, ct);
    }
}
