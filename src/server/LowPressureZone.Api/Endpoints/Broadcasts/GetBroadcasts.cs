using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

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
        {
            if (broadcastResult.Error is not null)
                ThrowError(broadcastResult.Error.ReasonPhrase ?? "Unknown reason",
                           (int)broadcastResult.Error.StatusCode);

            await SendForbiddenAsync(ct);
        }

        var broadcasts = broadcastResult.Value;
        if (broadcasts is null || broadcasts.Length == 0)
        {
            await SendOkAsync(ct);
            return;
        }

        var orderedBroadcasts = broadcasts.OrderBy(broadcast => broadcast.TimestampStart);
        var minBroadcastsStart = broadcasts.Min(broadcast => broadcast.TimestampStart);
        var maxBroadcastsStart = broadcasts.Max(broadcast => broadcast.TimestampStart);

        var minTimeslotStart = minBroadcastsStart.AddHours(-1);
        var maxTimeslotStart = maxBroadcastsStart.AddHours(1);
        var timeslots = await context.Timeslots
            .Where(timeslot => timeslot.StartsAt > minTimeslotStart && timeslot.EndsAt < maxTimeslotStart)
            .Include(timeslot => timeslot.Performer)
            .ToListAsync(ct);

        var responses = orderedBroadcasts.Select(Map.FromEntity);
        var broadcastResponses = responses as BroadcastResponse[] ?? responses.ToArray();
        foreach (var response in broadcastResponses)
        {
            var overlappingSlot = timeslots.FirstOrDefault(timeslot =>
                                                               timeslot.StartsAt.AddMinutes(-BroadcastBufferMinutes) <
                                                               response.Start
                                                               && timeslot.StartsAt.AddMinutes(BroadcastBufferMinutes) >
                                                               response.Start);
            if (overlappingSlot is null) continue;
            response.NearestPerformerName = overlappingSlot.Performer.Name;
        }

        await SendOkAsync(broadcastResponses, ct);
    }
}