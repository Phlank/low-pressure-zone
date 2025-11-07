using LowPressureZone.Adapter.AzuraCast;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Utilities;

namespace LowPressureZone.Api.Services.Hosted;

public class BroadcastDeletionService(AzuraCastClient client, ILogger<BroadcastDeletionService> logger)
    : IHostedService, IDisposable
{
    private const int RunHour = 5;
    private const int DaysToKeep = 14;
    private Timer? _timer;
    private static DateTime CutoffDate => DateTime.UtcNow.AddDays(-DaysToKeep);

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var nextRun = DateTimeUtilities.GetNextHour(RunHour);
        var spanToNext = nextRun - DateTime.UtcNow;
        var spanPeriod = new TimeSpan(24, 0, 0);
        logger.LogInformation("Broadcast deletion service started, first deletion in {Minutes} minutes",
                              spanToNext.TotalMinutes);
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
        logger.LogInformation("Deleting broadcasts older than {Days} days", DaysToKeep);
        var broadcastResult = await client.GetBroadcastsAsync();
        if (!broadcastResult.IsSuccess)
        {
            logger.LogError("Failed to delete broadcasts: {Reason}",
                            broadcastResult.Error.ReasonPhrase);
            return;
        }

        var outOfDateBroadcasts = broadcastResult.Value
                                                 .Where(broadcast => broadcast.TimestampStart < CutoffDate)
                                                 .ToList();
        logger.LogInformation("{OutOfDateBroadcasts} broadcasts out of date will be deleted",
                              outOfDateBroadcasts.Count);
        foreach (var broadcast in outOfDateBroadcasts)
        {
            if (broadcast.Streamer?.Id == null)
            {
                logger.LogWarning("Cannot delete broadcast {Id} because no streamer was returned",
                                  broadcast.Id);
                continue;
            }

            var deleteResult = await client.DeleteBroadcastAsync(broadcast.Streamer.Id,
                                                                 broadcast.Id);
            if (deleteResult.IsSuccess)
                continue;

            logger.LogError("Error when deleting broadcast {Id}: {Status}, {Reason}",
                            broadcast.Id,
                            deleteResult.Error.StatusCode,
                            deleteResult.Error.ReasonPhrase);
        }

        logger.LogInformation("Finished deleting broadcasts");
    }
}
