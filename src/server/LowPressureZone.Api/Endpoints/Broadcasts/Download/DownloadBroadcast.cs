using System.Globalization;
using System.Net;
using FastEndpoints;
using FluentValidation.Results;
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

        try
        {
            await Task.WhenAll(getStreamer, getBroadcasts);
        }
        catch (HttpRequestException exception)
        {
            Logger.LogError(exception, $"{nameof(DownloadBroadcast)}: Failed to retrieve streamer information");
        }

        var getStreamerResult = getStreamer.Result;
        var getBroadcastsResult = getBroadcasts.Result;

        if (!getStreamerResult.IsSuccess || !getBroadcastsResult.IsSuccess)
        {
            if (getStreamerResult.Error?.StatusCode == HttpStatusCode.NotFound)
                AddError(new ValidationFailure(nameof(req.StreamerId), "Streamer not found"));
            if (getBroadcastsResult.Error?.StatusCode == HttpStatusCode.NotFound)
                AddError(new ValidationFailure(nameof(req.StreamerId), "Streamer broadcasts not found"));
            ThrowIfAnyErrors();

            await SendNotFoundAsync(ct);
            return;
        }

        var broadcast = getBroadcastsResult.Value.FirstOrDefault(broadcast => broadcast.Id == req.BroadcastId);
        if (broadcast is null)
        {
            await SendNotFoundAsync(ct);
            Logger.LogError($"{nameof(DownloadBroadcast)}: Broadcast not found");
            return;
        }

        if (string.IsNullOrEmpty(broadcast.Recording?.DownloadUrl))
        {
            await SendNotFoundAsync(ct);
            Logger.LogError($"{nameof(DownloadBroadcast)}: Broadcast recording not available for download");
            return;
        }

        var fileName =
            $"{getStreamerResult.Value?.DisplayName ?? getStreamerResult.Value?.StreamerUsername ?? "Unknown DJ"} {broadcast.TimestampStart.ToString("yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture)}.mp3";

        var downloadResult = await client.DownloadBroadcastAsync(req.StreamerId, req.BroadcastId);
        if (!downloadResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            Logger.LogError($"{nameof(DownloadBroadcast)}: Broadcast download did not succeed");
            return;
        }

        var size = downloadResult.Value.Headers.ContentLength ?? 0;
        var stream = await downloadResult.Value.ReadAsStreamAsync(ct);
        await SendStreamAsync(stream, fileName, cancellation: ct, fileLengthBytes: size);
    }
}