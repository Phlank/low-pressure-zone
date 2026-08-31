using System.Globalization;
using System.Text;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Archive;

public class PostArchiveBroadcast(
    AzuraCastBroadcastDownloader downloader,
    AzuraCastMediaUploader uploader,
    AzuraCastMediaUpdater mediaUpdater,
    IAzuraCastClient azuraCastClient,
    DataContext dataContext,
    BroadcastPermissions permissions,
    IOptions<AzuraCastInstallationConfiguration> installationConfiguration) : Endpoint<ArchiveBroadcastRequest>
{
    private static readonly CompositeFormat ArchiveError = CompositeFormat.Parse("Unable to archive broadcast: {0}");
    private readonly string _archivePlaylistName = installationConfiguration.Value.ArchivePlaylistName;

    public override void Configure()
    {
        Post("/broadcasts/archive");
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ArchiveBroadcastRequest req, CancellationToken ct)
    {
        var broadcast = await dataContext.Broadcasts.FirstOrDefaultAsync(bc => bc.AzuraCastBroadcastId == req.Id, ct);
        if (broadcast is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!permissions.IsArchivable(broadcast))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var domainResult = broadcast.Archive();
        this.ThrowIfDomainError(domainResult);

        var externalBroadcastsResult = await azuraCastClient.GetBroadcastsAsync(broadcast.AzuraCastStreamerId);
        if (externalBroadcastsResult.IsError)
            ThrowError("Failed to retrieve broadcast from AzuraCast.", 500);

        var externalBroadcast = externalBroadcastsResult.Value.FirstOrDefault(b => b.Id == req.Id);
        if (externalBroadcast is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl))
            ThrowError(nameof(req.Id), "Broadcast recording is not available");

        if (broadcast is { IsArchived: true })
            ThrowError(nameof(req.Id), "Broadcast is already archived.");

        var playlistResult = await azuraCastClient.GetPlaylistByNameAsync(_archivePlaylistName);
        this.ThrowIfError(playlistResult, ArchiveError);
        var archivesPlaylist = playlistResult.Value;

        var recordingStreamResult = await downloader.GetStreamAsync(externalBroadcast.Id);
        this.ThrowIfError(recordingStreamResult, ArchiveError);
        await using var stream = recordingStreamResult.Value;

        var uploadResult = await uploader.UploadAndGetMediaAsync(stream, AzuraCastMediaDirectory.Archives);
        this.ThrowIfError(uploadResult, ArchiveError);
        var media = uploadResult.Value;

        var updateResult = await mediaUpdater.UpdateAsync(media,
                                                          externalBroadcast.Streamer!.DisplayName,
                                                          externalBroadcast.TimestampStart
                                                                           .ToString("yyyy-MM-dd",
                                                                                     CultureInfo.InvariantCulture),
                                                          [archivesPlaylist.Id]);
        this.ThrowIfError(updateResult, ArchiveError);

        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}