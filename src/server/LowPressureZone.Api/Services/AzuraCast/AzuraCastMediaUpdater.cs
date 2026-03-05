using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.AzuraCast;

public class AzuraCastMediaUpdater(IAzuraCastClient client)
{
    public async Task<Result<string>> UpdateAsync(
        StationMedia media,
        string artist,
        string title,
        List<int> playlistIds)
    {
        var updateRequest = StationMediaMapper.ToRequest(media);
        updateRequest.Artist = artist;
        updateRequest.Title = title;
        updateRequest.Playlists = playlistIds;
        var updateResult = await client.PutMediaAsync(media.Id, updateRequest);
        if (updateResult.IsError)
            return Result.Err("Failed to update media");
        return new Result<string>();
    }
}