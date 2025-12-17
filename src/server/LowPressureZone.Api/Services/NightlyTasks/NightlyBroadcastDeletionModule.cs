using System.Net;
using LowPressureZone.Adapter.AzuraCast.Clients;

namespace LowPressureZone.Api.Services.NightlyTasks;

public sealed partial class NightlyBroadcastDeletionModule(IAzuraCastClient client, ILogger<NightlyBroadcastDeletionModule> logger)
{
    private const int DaysToKeep = 14;
    private static DateTime CutoffDate => DateTime.UtcNow.AddDays(-DaysToKeep);
    
    public async Task DeleteOutOfDateBroadcastsAsync()
    {
        LogDeletingBroadcasts(logger, DaysToKeep);
        var broadcastResult = await client.GetBroadcastsAsync();
        if (!broadcastResult.IsSuccess)
        {
            LogFailedToDeleteBroadcastsReason(logger, broadcastResult.Error.ReasonPhrase ?? "Unknown reason");
            return;
        }

        var outOfDateBroadcasts = broadcastResult.Value
                                                 .Where(broadcast => broadcast.TimestampStart < CutoffDate)
                                                 .ToList();
        LogBroadcastsOutOfDateWillBeDeleted(logger, outOfDateBroadcasts.Count);
        foreach (var broadcast in outOfDateBroadcasts)
        {
            if (broadcast.Streamer?.Id == null)
            {
                LogBroadcastDeleteFailedNoStreamer(logger, broadcast.Id);
                continue;
            }

            var deleteResult = await client.DeleteBroadcastAsync(broadcast.Streamer.Id,
                                                                 broadcast.Id);
            if (deleteResult.IsSuccess)
                continue;

            LogFailedToDeleteBroadcast(logger, broadcast.Id, deleteResult.Error.StatusCode,
                                       deleteResult.Error.ReasonPhrase ?? "Unknown reason");
        }

        LogFinishedDeletingBroadcasts(logger);
    }
    
    [LoggerMessage(LogLevel.Information, "Deleting broadcasts older than {days} days")]
    static partial void LogDeletingBroadcasts(ILogger<NightlyBroadcastDeletionModule> logger, int days);

    [LoggerMessage(LogLevel.Error, "Failed to delete broadcasts: {reason}")]
    static partial void LogFailedToDeleteBroadcastsReason(ILogger<NightlyBroadcastDeletionModule> logger, string reason);

    [LoggerMessage(LogLevel.Information, "{outOfDateBroadcasts} broadcasts out of date will be deleted")]
    static partial void LogBroadcastsOutOfDateWillBeDeleted(
        ILogger<NightlyBroadcastDeletionModule> logger,
        int outOfDateBroadcasts);

    [LoggerMessage(LogLevel.Warning, "Cannot delete broadcast {id} because no streamer was returned")]
    static partial void LogBroadcastDeleteFailedNoStreamer(ILogger<NightlyBroadcastDeletionModule> logger, int id);

    [LoggerMessage(LogLevel.Error, "Error when deleting broadcast {id}: {status}, {reason}")]
    static partial void LogFailedToDeleteBroadcast(
        ILogger<NightlyBroadcastDeletionModule> logger,
        int id,
        HttpStatusCode status,
        string reason);

    [LoggerMessage(LogLevel.Information, "Finished deleting broadcasts")]
    static partial void LogFinishedDeletingBroadcasts(ILogger<NightlyBroadcastDeletionModule> logger);
}