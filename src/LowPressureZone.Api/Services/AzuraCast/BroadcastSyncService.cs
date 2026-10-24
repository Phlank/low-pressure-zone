using FastEndpoints;
using LowPressureZone.Api.Commands.Broadcasts.SyncBroadcasts;
using LowPressureZone.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.Services.AzuraCast;

public class BroadcastSyncService(IServiceScopeFactory scopeFactory, ILogger<BroadcastSyncService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                logger.LogInformation("Synchronizing broadcasts...");
                using var scope = scopeFactory.CreateScope();
                var syncBroadcastsHandler = scope.ServiceProvider.GetRequiredService<SyncBroadcastsHandler>();
                var command = new SyncBroadcastsCommand();
                await syncBroadcastsHandler.ExecuteAsync(command, stoppingToken);
                logger.LogInformation("Broadcast synchronization completed successfully.");
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while synchronizing broadcasts; attempting on next loop");
            }
        }
    }
}