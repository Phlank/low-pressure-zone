using System.Net;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Utilities;

namespace LowPressureZone.Api.Services;

public sealed partial class BroadcastDeletionService(IAzuraCastClient client, ILogger<BroadcastDeletionService> logger)
    : IHostedService, IDisposable
{
    private const int RunHour = 5;
    private const int DaysToKeep = 14;
    private Timer? _timer;
    private static DateTime CutoffDate => DateTime.UtcNow.AddDays(-DaysToKeep);

    public void Dispose() => _timer?.Dispose();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var nextRun = DateTimeUtilities.GetNextHour(RunHour);
        var spanToNext = nextRun - DateTime.UtcNow;
        var spanPeriod = new TimeSpan(24, 0, 0);
        LogBroadcastDeletionServiceStarted(logger, spanToNext.TotalMinutes);
        _timer = new Timer(RunDelete, null, spanToNext, spanPeriod);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    private void RunDelete(object? value) => _ = DeleteOutOfDateBroadcasts();

    private async Task DeleteOutOfDateBroadcasts()
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

    [LoggerMessage(LogLevel.Information, "Broadcast deletion service started, first deletion in {minutes} minutes")]
    static partial void LogBroadcastDeletionServiceStarted(ILogger<BroadcastDeletionService> logger, double minutes);

    [LoggerMessage(LogLevel.Information, "Deleting broadcasts older than {days} days")]
    static partial void LogDeletingBroadcasts(ILogger<BroadcastDeletionService> logger, int days);

    [LoggerMessage(LogLevel.Error, "Failed to delete broadcasts: {reason}")]
    static partial void LogFailedToDeleteBroadcastsReason(ILogger<BroadcastDeletionService> logger, string reason);

    [LoggerMessage(LogLevel.Information, "{outOfDateBroadcasts} broadcasts out of date will be deleted")]
    static partial void LogBroadcastsOutOfDateWillBeDeleted(
        ILogger<BroadcastDeletionService> logger,
        int outOfDateBroadcasts);

    [LoggerMessage(LogLevel.Warning, "Cannot delete broadcast {id} because no streamer was returned")]
    static partial void LogBroadcastDeleteFailedNoStreamer(ILogger<BroadcastDeletionService> logger, int id);

    [LoggerMessage(LogLevel.Error, "Error when deleting broadcast {id}: {status}, {reason}")]
    static partial void LogFailedToDeleteBroadcast(
        ILogger<BroadcastDeletionService> logger,
        int id,
        HttpStatusCode status,
        string reason);

    [LoggerMessage(LogLevel.Information, "Finished deleting broadcasts")]
    static partial void LogFinishedDeletingBroadcasts(ILogger<BroadcastDeletionService> logger);
}