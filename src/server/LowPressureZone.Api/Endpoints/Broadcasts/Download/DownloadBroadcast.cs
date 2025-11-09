using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Download;

public class DownloadBroadcast(AzuraCastClient client, UserManager<AppUser> userManager)
    : Endpoint<DownloadBroadcastRequest, IFormFile>
{
    public override void Configure() => Get("/broadcasts/download");

    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates",
                     Justification = "Not performance sensitive")]
    public override async Task HandleAsync(DownloadBroadcastRequest req, CancellationToken ct)
    {
        if (!(User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Organizer)))
        {
            var user = await userManager.GetUserAsync(User);
            if (user?.StreamerId != req.StreamerId)
            {
                await SendUnauthorizedAsync(ct);
                return;
            }
        }

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

        var downloadResult = await client.DownloadBroadcastFileAsync(req.StreamerId, req.BroadcastId);
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