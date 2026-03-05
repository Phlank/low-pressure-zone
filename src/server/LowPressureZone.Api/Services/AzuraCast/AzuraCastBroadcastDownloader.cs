using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.AzuraCast;

public class AzuraCastBroadcastDownloader(IAzuraCastClient azuraCastClient)
{
    public async Task<Result<Stream, string>> GetStreamAsync(int broadcastId)
    {
        var externalBroadcastsResult = await azuraCastClient.GetBroadcastsAsync();
        if (externalBroadcastsResult.IsError)
            return Result.Err<Stream>("Unable to retrieve broadcasts from AzuraCast.");

        var externalBroadcast = externalBroadcastsResult.Value.FirstOrDefault(broadcast => broadcast.Id == broadcastId);
        if (externalBroadcast is null)
            return Result.Err<Stream>("Could not find broadcast.");

        if (string.IsNullOrEmpty(externalBroadcast.Recording?.DownloadUrl))
            return Result.Err<Stream>("Broadcast recording is not downloadable.");

        var downloadResult = await azuraCastClient.DownloadBroadcastFileAsync(externalBroadcast.Streamer!.Id, 
                                                                              externalBroadcast.Id);
        if (downloadResult.IsError)
            return Result.Err<Stream>("Could not retrieve download.");

        return Result.Ok(await downloadResult.Value.ReadAsStreamAsync());
    }
}