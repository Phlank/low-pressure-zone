using System.Globalization;
using System.Text;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Resolvers;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Permissions;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Data;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Infrastructure;
using LowPressureZone.Infrastructure.Adapters;
using LowPressureZone.Infrastructure.Configuration;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Archive;

public class PostArchiveBroadcast(
    AzuraCastBroadcastDownloader downloader,
    IAzuraCastClient azuraCastClient,
    IAzuraCastMediaPathResolver pathResolver,
    IS3Adapter s3,
    IDbContextOutbox<DataContext> outbox,
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
        var broadcast = await outbox.DbContext
                                    .Broadcasts
                                    .FirstOrDefaultAsync(bc => bc.AzuraCastBroadcastId == req.Id, ct);
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

        var externalBroadcastsResult = await azuraCastClient.GetBroadcastsAsync(broadcast.AzuraCastStreamerId);
        if (externalBroadcastsResult.IsError)
            ThrowError("Failed to retrieve broadcast from AzuraCast.", 500);

        var streamerResult = await azuraCastClient.GetStreamerAsync(broadcast.AzuraCastStreamerId);
        if (streamerResult.IsError || streamerResult.Value.DisplayName is null)
            ThrowError("Failed to retrieve streamer from AzuraCast.", 500);

        var externalBroadcast = externalBroadcastsResult.Value.FirstOrDefault(b => b.Id == req.Id);
        if (externalBroadcast is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl))
            ThrowError(r => r.Id, "Broadcast recording is not available");

        var recordingStreamResult = await downloader.GetStreamAsync(externalBroadcast.Id);
        this.ThrowIfError(recordingStreamResult, ArchiveError);
        
        await using var stream = recordingStreamResult.Value;
        var fileName = MixFile.GetMixFileName(streamerResult.Value.DisplayName, externalBroadcast.TimestampStart);
        var path = pathResolver.ResolveFilePath(AzuraCastMediaDirectory.Archives, fileName);
        var uploadResult = await s3.PutObjectAsync(stream, path, ct);
        if (uploadResult.HttpStatusCode != System.Net.HttpStatusCode.OK)
            ThrowError("Unable to upload media.", 500);
        
        var uploadedMediaResult = UploadedMedia.Create(fileName, path);
        this.ThrowIfDomainErrors(uploadedMediaResult);
        
        outbox.DbContext.UploadedMedia.Add(uploadedMediaResult.Value);
        await outbox.PublishAllAsync(uploadedMediaResult.Events);
        
        var domainResult = broadcast.Archive(uploadedMediaResult.Value.Id);
        this.ThrowIfDomainErrors(domainResult);
        await outbox.PublishAllAsync(domainResult.Events);
        
        await outbox.SaveChangesAndFlushMessagesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}