using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Adapter.AzuraCast.Mappers;
using LowPressureZone.Core;
using Results = LowPressureZone.Core.Results;

namespace LowPressureZone.Api.Services.AzuraCast;

[RegisterService<AzuraCastMediaUpdater>(LifeTime.Singleton)]
public class AzuraCastMediaUpdater(ILogger<AzuraCastMediaUpdater> logger, IAzuraCastClient client)
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
        {
            logger.LogError("Failed to update media: {Reason}", updateResult.Error.ReasonPhrase);
            return Results.Err("Failed to update media");
        }
        return new Result<string>();
    }
}