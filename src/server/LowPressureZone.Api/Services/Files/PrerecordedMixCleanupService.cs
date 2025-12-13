using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.Files;

public sealed class PrerecordedMixCleanupService(IAzuraCastClient azuraCastClient, ILogger<PrerecordedMixCleanupService> logger)
{
    public async Task<Result<bool, string>> DeleteEnqueuedPrerecordedMixAsync(int azuraCastMediaId)
    {
        var getMediaResult = await azuraCastClient.GetMediaAsync(azuraCastMediaId);
        if (getMediaResult.IsError)
            return Result.Err<bool>("Unable to retrieve media in AzuraCast");

        var mediaId = getMediaResult.Value.Id;
        var isPlaylistDeleteError = false;
        var playlistId = getMediaResult.Value.Playlists.FirstOrDefault()?.Id;
        if (playlistId is not null)
        {
            var deletePlaylistResult = await azuraCastClient.DeletePlaylistAsync(playlistId.Value);
            if (deletePlaylistResult.IsError)
                isPlaylistDeleteError = true;
        }

        var deleteMediaResult = await azuraCastClient.DeleteMediaAsync(mediaId);
        if (deleteMediaResult.IsError)
            return Result.Err<bool>("Failed to delete media in AzuraCast");
        
        if (isPlaylistDeleteError)
            logger.LogWarning("Failed to delete AzuraCast playlist for timeslot, but successfully deleted the media.");

        return Result.Ok(true);
    }
}