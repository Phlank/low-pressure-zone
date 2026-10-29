using LowPressureZone.Api.Utilities;
using LowPressureZone.Infrastructure.Emails;

namespace LowPressureZone.Api.Services.NightlyTasks;

public sealed partial class NightlyTaskService(
    IEmailService emailService,
    NightlyBroadcastDeletionModule nightlyBroadcastDeletionModule,
    NightlyPrerecordedMixCleanupModule nightlyPrerecordedMixCleanupModule,
    ILogger<NightlyTaskService> logger)
    : BackgroundService
{
    private const int RunHour = 5;

    private static TimeSpan TimeSpanToFirst => DateTimeUtilities.GetNextHour(RunHour) - DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timeToFirstRun = TimeSpanToFirst;
        using var timer = new PeriodicTimer(timeToFirstRun);
        var isFirstRun = true;
        LogNightlyTaskServiceStarted(logger, timeToFirstRun.TotalMinutes);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            try
            {
                if (isFirstRun)
                {
                    isFirstRun = false;
                    timer.Period = TimeSpan.FromDays(1);
                }

                await nightlyBroadcastDeletionModule.DeleteOutOfDateBroadcastsAsync();
                await nightlyPrerecordedMixCleanupModule.CleanupPrerecordedMixesAsync();
            }
            catch (Exception ex)
            {
                await emailService.SendAdminException(ex, "Nightly tasks");
                logger.LogError(ex, "Exception thrown while executing nightly tasks");
            }
    }

    [LoggerMessage(LogLevel.Information, "Nightly task service started, first run in {minutes} minutes")]
    static partial void LogNightlyTaskServiceStarted(ILogger<NightlyTaskService> logger, double minutes);
}