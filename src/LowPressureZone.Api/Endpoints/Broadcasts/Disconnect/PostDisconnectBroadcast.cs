using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity;
using LowPressureZone.Identity.Application.Commands.EnableStreamer;
using LowPressureZone.Identity.Application.Jobs;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Disconnect;

public class PostDisconnectBroadcast(IAzuraCastClient client, IdentityContext identityContext, IScheduler scheduler)
    : Endpoint<PostDisconnectBroadcastRequest>
{
    public override void Configure()
    {
        Post("/broadcasts/disconnect");
        Roles(RoleNames.Admin, RoleNames.Organizer);
        Description(builder => builder.Produces(204)
                                      .Produces(400));
    }

    public override async Task HandleAsync(PostDisconnectBroadcastRequest req, CancellationToken ct)
    {
        if (req.DisableMinutes.HasValue)
        {
            await DisableStreamer(req.DisableMinutes.Value);
        }

        var response = await client.PostBroadcastingAction(BroadcastingActionType.Disconnect);
        if (!response.IsSuccess)
            ThrowError($"Error returned from AzuraCast API: {response.Error.ReasonPhrase ?? response.Error.StatusCode.ToString()}");

        await Send.NoContentAsync(ct);
    }

    private async Task DisableStreamer(int minutes)
    {
        var broadcastsResult = await client.GetBroadcastsAsync();
        var currentBroadcast = broadcastsResult.Value.FirstOrDefault(broadcast => broadcast.TimestampEnd is null);
        if (currentBroadcast is null)
            ThrowError("No broadcast is currently active.");

        var streamerId = currentBroadcast.Streamer?.Id;
        if (!streamerId.HasValue)
            ThrowError("Current broadcast does not have an associated streamer.");

        var disableRequest = await client.DisableStreamerAsync(streamerId.Value);
        if (!disableRequest.IsSuccess)
            ThrowError($"Error disabling streamer: {disableRequest.Error.ReasonPhrase ?? disableRequest.Error.StatusCode.ToString()}");

        var userId = await identityContext.Users
                                          .AsNoTracking()
                                          .Where(u => u.StreamerId == streamerId.Value)
                                          .Select(u => u.Id)
                                          .FirstOrDefaultAsync();

        if (minutes > 0 && userId != Guid.Empty)
        {
            var timespan = TimeSpan.FromMinutes(minutes);
            var enableTime = DateTimeOffset.UtcNow.Add(timespan);
            var options = OneOffJobOptions.Replacing(EnableStreamerJob.JobKey(userId));
            await scheduler.ScheduleJob<IJob<EnableStreamerCommand>, EnableStreamerCommand>(new(userId),
                                                                                            enableTime,
                                                                                            options);
        }
    }
}