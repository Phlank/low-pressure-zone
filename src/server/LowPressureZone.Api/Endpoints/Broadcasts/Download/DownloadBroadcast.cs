using System.Globalization;
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
        var getStreamer = client.GetStreamerAsync(req.StreamerId);
        var getBroadcasts = client.GetBroadcastsAsync(req.StreamerId);
        await Task.WhenAll(getStreamer, getBroadcasts);

        var getStreamerResult = getStreamer.Result;
        var getBroadcastsResult = getBroadcasts.Result;

        if (!getStreamerResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

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
        var fileName = $"{getStreamerResult.Data?.DisplayName ?? getStreamerResult.Data?.StreamerUsername ?? "Unknown DJ"} {broadcast.TimestampStart.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture)}.mp3";

        var downloadResult = await client.DownloadBroadcastAsync(req.StreamerId, req.BroadcastId);
        if (!downloadResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        var size = downloadResult.Data!.Headers.ContentLength ?? 0;
        var stream = await downloadResult.Data!.ReadAsStreamAsync(ct);
        await SendStreamAsync(stream, fileName, cancellation: ct, fileLengthBytes: size);
    }
}
