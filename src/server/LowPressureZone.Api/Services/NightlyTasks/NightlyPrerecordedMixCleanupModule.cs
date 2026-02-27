using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Services.NightlyTasks;

public sealed partial class NightlyPrerecordedMixCleanupModule(
    IServiceProvider services,
    PrerecordedMixCleanupService cleanupService,
    ILogger<NightlyPrerecordedMixCleanupModule> logger)
{
    private static DateTimeOffset CutoffTime => DateTimeOffset.UtcNow.AddHours(-1);

    public async Task CleanupPrerecordedMixesAsync()
    {
        logger.LogInformation("Starting nightly prerecorded mix cleanup task");
        await using var scope = services.CreateAsyncScope();
        await using var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

        var prerecordedTimeslots = await dataContext.Timeslots
                                                    .Where(timeslot => timeslot.Type == PerformanceTypes.Prerecorded
                                                                       && timeslot.AzuraCastMediaId.HasValue
                                                                       && timeslot.EndsAt <= CutoffTime)
                                                    .ToListAsync();

        LogDeletingMediaAndPlaylistsForTimeslotCountPastTimeslots(logger, prerecordedTimeslots.Count);
        foreach (var timeslot in prerecordedTimeslots)
        {
            var mediaId = timeslot.AzuraCastMediaId!.Value;
            try
            {
                var deleteResult = await cleanupService.DeleteEnqueuedPrerecordedMixAsync(mediaId);
                if (deleteResult.IsError)
                    logger.LogError("Unable to delete prerecorded mix items in AzuraCast for timeslot {TimeslotId}",
                                    timeslot.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception thrown while deleting prerecorded mix items in AzuraCast");
            }

            timeslot.AzuraCastMediaId = null;
        }

        await dataContext.SaveChangesAsync();
        logger.LogInformation("Finished deleting media and playlists for past prerecorded timeslots");
    }

    [LoggerMessage(LogLevel.Information, "Deleting media and playlists for {timeslotCount} past timeslots")]
    static partial void LogDeletingMediaAndPlaylistsForTimeslotCountPastTimeslots(ILogger<NightlyPrerecordedMixCleanupModule> logger, int timeslotCount);
}