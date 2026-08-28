using FastEndpoints;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Services.NightlyTasks;

[RegisterService<NightlyPrerecordedMixCleanupModule>(LifeTime.Singleton)]
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

        var prerecordedSlots = await dataContext.HourlySlots
                                                    .Where(hourlySlot => hourlySlot.Prerecord.AzuraCastMediaId.HasValue
                                                                         && hourlySlot.EndsAt <= CutoffTime)
                                                    .ToListAsync();

        LogDeletingMediaAndPlaylistsForTimeslotCountPastTimeslots(logger, prerecordedSlots.Count);
        foreach (var slot in prerecordedSlots)
        {
            var mediaId = slot.Prerecord.AzuraCastMediaId!.Value;
            try
            {
                var deleteResult = await cleanupService.DeleteEnqueuedPrerecordedMixAsync(mediaId);
                if (deleteResult.IsError)
                    logger.LogError("Unable to delete prerecorded mix items in AzuraCast for timeslot {TimeslotId}",
                                    slot.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception thrown while deleting prerecorded mix items in AzuraCast");
            }

            var deleteMixResult = slot.DeletePrerecordedMix();
        }

        await dataContext.SaveChangesAsync();
        logger.LogInformation("Finished deleting media and playlists for past prerecorded timeslots");
    }

    [LoggerMessage(LogLevel.Information, "Deleting media and playlists for {timeslotCount} past timeslots")]
    static partial void LogDeletingMediaAndPlaylistsForTimeslotCountPastTimeslots(
        ILogger<NightlyPrerecordedMixCleanupModule> logger,
        int timeslotCount);
}