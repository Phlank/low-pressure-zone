using System.Globalization;
using System.Text;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Core;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Archive;

public class PostArchiveBroadcast(
    AzuraCastBroadcastDownloader downloader,
    AzuraCastMediaUploader uploader,
    AzuraCastMediaUpdater updater,
    IAzuraCastClient azuraCastClient,
    DataContext dataContext,
    BroadcastRules rules,
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
        var externalBroadcastsResult = await azuraCastClient.GetBroadcastsAsync();
        if (externalBroadcastsResult.IsError)
            ThrowError("Failed to retrieve broadcast from AzuraCast.", 500);

        var externalBroadcast = externalBroadcastsResult.Value.FirstOrDefault(broadcast => broadcast.Id == req.Id);
        if (externalBroadcast is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var broadcast = await dataContext.Broadcasts
                                         .Where(broadcast => broadcast.AzuraCastBroadcastId == req.Id)
                                         .FirstOrDefaultAsync(ct);

        if (string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl))
            ThrowError(nameof(req.Id), "Broadcast recording is not available");

        if (broadcast is { IsArchived: true })
            ThrowError(nameof(req.Id), "Broadcast is already archived.");

        if (!rules.IsArchivable(externalBroadcast, broadcast))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var playlistResult = await azuraCastClient.GetPlaylistByNameAsync(_archivePlaylistName);
        this.ThrowIfError(playlistResult, ArchiveError);
        var archivesPlaylist = playlistResult.Value;

        var recordingStreamResult = await downloader.GetStreamAsync(externalBroadcast.Id);
        this.ThrowIfError(recordingStreamResult, ArchiveError);
        await using var stream = recordingStreamResult.Value;

        var uploadResult = await uploader.UploadAndGetMediaAsync(stream, AzuraCastMediaDirectory.Archives);
        this.ThrowIfError(uploadResult, ArchiveError);
        var media = uploadResult.Value;

        var updateResult = await updater.UpdateAsync(media,
                                                     externalBroadcast.Streamer!.DisplayName,
                                                     externalBroadcast.TimestampStart
                                                                      .ToString("yyyy-MM-dd",
                                                                                CultureInfo.InvariantCulture),
                                                     [archivesPlaylist.Id]);
        this.ThrowIfError(updateResult, ArchiveError);
        
        if (broadcast is not null)
        {
            broadcast.IsArchived = true;
            broadcast.LastModifiedDate = DateTime.UtcNow;
        }
        else
        {
            dataContext.Broadcasts.Add(new()
            {
                AzuraCastBroadcastId = externalBroadcast.Id,
                IsArchived = true
            });
        }

        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}