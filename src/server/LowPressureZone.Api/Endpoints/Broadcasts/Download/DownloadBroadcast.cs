using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Download;

public class DownloadBroadcast(AzuraCastClient client) : Endpoint<DownloadBroadcastRequest, IFormFile>
{
    public override void Configure()
    {
        Get("/broadcasts/download");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(DownloadBroadcastRequest req, CancellationToken ct)
    {
        var getBroadcastsResult = await client.GetBroadcastsAsync(req.StreamerId);
        if (!getBroadcastsResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var broadcast = getBroadcastsResult.Data!.FirstOrDefault(broadcast => broadcast.Id == req.BroadcastId);
        if (broadcast is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var downloadResult = await client.DownloadBroadcastAsync(req.StreamerId, req.BroadcastId);
        if (!downloadResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendStreamAsync(downloadResult.Data!, cancellation: ct);
    }
}
