using LowPressureZone.Api.Utilities;

namespace LowPressureZone.Api.Services.NightlyTasks;

public sealed partial class NightlyTaskService(
    EmailService emailService,
    NightlyBroadcastDeletionModule nightlyBroadcastDeletionModule,
    NightlyPrerecordedMixCleanupModule nightlyPrerecordedMixCleanupModule,
    ILogger<NightlyTaskService> logger)
    : IHostedService, IDisposable
{
    private const int RunHour = 5;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(5));
    // private readonly PeriodicTimer _timer = new(TimeSpanToFirst);

    private bool _isFirstTick = true;
    private bool _isStarted;

    private static TimeSpan TimeSpanToFirst => DateTimeUtilities.GetNextHour(RunHour) - DateTime.UtcNow;

    public void Dispose() => _timer.Dispose();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _isStarted = true;
        _isFirstTick = true;
        _ = ContinuallyExecuteNightlyTasksAsync();
        LogNightlyTaskServiceStarted(logger, TimeSpanToFirst.TotalMinutes);

        return Task.CompletedTask;
    }

    private async Task ContinuallyExecuteNightlyTasksAsync()
    {
        while (await _timer.WaitForNextTickAsync() && _isStarted)
        {
            try
            {
                if (_isFirstTick)
                {
                    _isFirstTick = false;
                    _timer.Period = TimeSpan.FromDays(1);
                }

                await nightlyBroadcastDeletionModule.DeleteOutOfDateBroadcastsAsync();
                await nightlyPrerecordedMixCleanupModule.CleanupPrerecordedMixesAsync();
            }
            catch (Exception ex)
            {
                await emailService.SendAdminMessage("Exception in Nightly Tasks",
                                                    "An exception was thrown while executing nightly tasks. Please check the logs for more details.");
                logger.LogError(ex, "Exception thrown while executing nightly tasks");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _isStarted = false;
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Nightly task service started, first run in {minutes} minutes")]
    static partial void LogNightlyTaskServiceStarted(ILogger<NightlyTaskService> logger, double minutes);
}