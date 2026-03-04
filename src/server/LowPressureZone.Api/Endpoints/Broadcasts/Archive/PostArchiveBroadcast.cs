using System.Globalization;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Broadcasts.Archive;

public class PostArchiveBroadcast(
    IAzuraCastClient azuraCastClient,
    DataContext dataContext,
    BroadcastRules rules,
    IOptions<AzuraCastInstallationConfiguration> installationConfiguration) : Endpoint<ArchiveBroadcastRequest>
{
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

        if (!rules.IsArchivable(externalBroadcast, broadcast))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl))
            ThrowError(nameof(req.Id), "Broadcast recording is not available");
        
        if (broadcast is { IsArchived: true })
            ThrowError(nameof(req.Id), "Broadcast is already archived.");

        var playlistsResult = await azuraCastClient.GetPlaylistsAsync();
        if (playlistsResult.IsError)
            ThrowError("Unable to get playlists from AzuraCast", 500);

        var archivesPlaylist = playlistsResult.Value
                                              .FirstOrDefault(playlist => playlist.Name ==
                                                                          installationConfiguration.Value
                                                                                                   .ArchivePlaylistName);
        if (archivesPlaylist is null)
            ThrowError("Could not find archive playlist in AzuraCast", 500);

        var downloadResult = await azuraCastClient.DownloadBroadcastFileAsync(externalBroadcast.Streamer!.Id,
                                                                              externalBroadcast.Id);
        if (downloadResult.IsError)
            ThrowError("Unable to download file from AzuraCast", 500);
        
        var stream = await downloadResult.Value.ReadAsStreamAsync(ct);

        var filePath = $"{installationConfiguration.Value.ArchiveSetLocation}/{Guid.NewGuid()}.mp3";
        var uploadResult = await azuraCastClient.UploadMediaViaSftpAsync(stream, filePath);
        if (uploadResult.IsError)
            ThrowError("Failed to upload file to AzuraCast", 500);

        var uploadedFileResult = await Retry.RetryAsync(async () => await GetUploadedFileAsync(filePath),
                                                        result => result.IsError
                                                                  || (result.IsSuccess
                                                                      && result.Value.Media is not null), 1000, 10, ct);
        if (uploadedFileResult.IsError)
            ThrowError("Failed to update file metadata in AzuraCast", 500);
        var uploadedFile = uploadedFileResult.Value;
        uploadedFile.Media.ShouldNotBeNull();
        var media = uploadedFile.Media;
        var updateMediaRequest = StationMediaMapper.ToRequest(media);
        updateMediaRequest.Artist = externalBroadcast.Streamer.DisplayName;
        updateMediaRequest.Title = externalBroadcast.TimestampStart
                                                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        updateMediaRequest.Playlists = [archivesPlaylist.Id];
        var updateMediaResult = await azuraCastClient.PutMediaAsync(media.Id, updateMediaRequest);
        if (updateMediaResult.IsError)
            ThrowError("Failed to update media metadata in AzuraCast", 500);

        if (broadcast is null)
        {
            broadcast = new Broadcast()
            {
                AzuraCastBroadcastId = externalBroadcast.Id,
                IsArchived = true
            };
            dataContext.Broadcasts.Add(broadcast);
        }
        else
        {
            broadcast.IsArchived = true;
            broadcast.LastModifiedDate = DateTime.UtcNow;
        }

        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }

    private async Task<Result<StationFileListItem, string>> GetUploadedFileAsync(string filePath)
    {
        var archiveListResult =
            await azuraCastClient.GetMediaInDirectoryAsync(installationConfiguration.Value.ArchiveSetLocation);

        if (archiveListResult.IsError)
            return Result.Err<StationFileListItem>("Failed to retrieve files from AzuraCast");

        var uploadedFile = archiveListResult.Value
                                            .FirstOrDefault(file => file.PathShort == filePath.Split('/').Last());

        if (uploadedFile is null)
            return Result.Err<StationFileListItem>("Uploaded file not found in AzuraCast prerecorded directory");

        return Result.Ok<StationFileListItem, string>(uploadedFile);
    }
}