using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Broadcasts;

public class GetBroadcasts(AzuraCastClient client, DataContext context)
    : EndpointWithoutRequest<IEnumerable<BroadcastResponse>, BroadcastMapper>
{
    private const int BroadcastBufferMinutes = 20;

    public override void Configure()
    {
        Get("/broadcasts");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var broadcastResult = await client.GetBroadcastsAsync();
        if (!broadcastResult.IsSuccess)
            ThrowError(broadcastResult.Error.ReasonPhrase ?? "Unknown reason",
                       (int)broadcastResult.Error.StatusCode);

        var broadcasts = broadcastResult.Value;
        if (broadcasts.Length == 0)
        {
            await SendOkAsync(ct);
            return;
        }

        var orderedBroadcasts = broadcasts.OrderBy(broadcast => broadcast.TimestampStart);

        var responses = orderedBroadcasts.Select(Map.FromEntity);
        var broadcastResponses = responses as BroadcastResponse[] ?? responses.ToArray();

        await SendOkAsync(broadcastResponses, ct);
    }
}
