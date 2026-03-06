using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
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

        var broadcastsResult = await client.GetBroadcastsAsync();

        if (!broadcastsResult.IsSuccess)
            ThrowError(broadcastsResult.Error.ReasonPhrase ?? "Unknown reason",
                       (int)broadcastsResult.Error.StatusCode);

        var externalBroadcasts = broadcastsResult.Value
                                                 .OrderByDescending(broadcast => broadcast.TimestampStart)
                                                 .ToList();
        Dictionary<int, Broadcast> broadcasts = new();
        if (!(User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)))
        {
            externalBroadcasts = externalBroadcasts.Where(broadcast => broadcast.Streamer?.Id == user.StreamerId)
                                                   .ToList();
        }
        else
        {
            var externalIds = externalBroadcasts.Select(broadcast => broadcast.Id);
            broadcasts = await dataContext.Broadcasts
                                          .Where(broadcast => externalIds.Contains(broadcast.AzuraCastBroadcastId))
                                          .ToDictionaryAsync(broadcast => broadcast.AzuraCastBroadcastId, ct);
        }

        List<BroadcastResponse> responses = new(externalBroadcasts.Count);
        foreach (var externalBroadcast in externalBroadcasts)
            responses.Add(Map.FromEntity(externalBroadcast, broadcasts.GetValueOrDefault(externalBroadcast.Id)));

        await Send.OkAsync(responses, ct);
    }
}