using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Exceptions;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Core;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Scheduling.Application.Commands.UploadedMedia.AddMediaToPlaylist;

public class AddMediaToPlaylistHandler(DataContext dataContext, IAzuraCastClient azuraCast)
    : ICommandHandler<AddMediaToPlaylist, Result<string>>
{
    public async Task<Result<string>> ExecuteAsync(AddMediaToPlaylist command, CancellationToken ct)
    {
        var uploadedMedia = await dataContext.UploadedMedia.FirstOrDefaultAsync(m => m.Id == command.Id, ct);
        if (uploadedMedia is null)
            return Results.Err("Uploaded media not found");

        if (!uploadedMedia.AzuraCastMediaId.HasValue || !uploadedMedia.AzuraCastPlaylistId.HasValue)
            return Results.Err("Uploaded media is not linked to AzuraCast media and playlist");

        var media = await azuraCast.GetMediaAsync(uploadedMedia.AzuraCastMediaId.Value);
        if (media.IsError)
            throw new IntegrationException("Unable to retrieve media from AzuraCast: " +
                                           (media.Error.ReasonPhrase ?? media.Error.StatusCode.ToString()));
        
        var putRequest = StationMediaMapper.ToRequest(media.Value);
        putRequest.Playlists = [uploadedMedia.AzuraCastPlaylistId.Value];
        var putResult = await azuraCast.PutMediaAsync(uploadedMedia.AzuraCastMediaId.Value, putRequest);
        if (putResult.IsError)
            throw new IntegrationException("Unable to update media in AzuraCast: " +
                                           (putResult.Error.ReasonPhrase ?? putResult.Error.StatusCode.ToString()));
        
        return Results.Ok<string>();
    }
}