using System.Globalization;
using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class DeleteTimeslot(
    DataContext dataContext,
    PrerecordedMixCleanupService cleanupService,
    TimeslotRules rules,
    EmailService emailService,
    ILogger<DeleteTimeslot> logger)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");
        var timeslot = await dataContext.Timeslots
                                        .AsNoTracking()
                                        .Include(t => t.Performer)
                                        .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                        .FirstOrDefaultAsync(ct);

        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsDeleteAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (timeslot.Type == PerformanceTypes.Prerecorded && timeslot.AzuraCastMediaId.HasValue)
        {
            var deleteResult = await cleanupService.DeleteEnqueuedPrerecordedMixAsync(timeslot.AzuraCastMediaId.Value);
            if (deleteResult.IsError)
            {
                logger.LogWarning("Failed to delete items in AzuraCast for prerecorded timeslot. Timeslot date and time: {Timestamp}",
                                  timeslot.StartsAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                _ = await emailService.SendAdminMessage("Failure to delete items in AzuraCast for prerecorded timeslot",
                                                        $"Failed to delete items in AzuraCast for prerecorded timeslot. Timeslot date and time: {timeslot.StartsAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}\n\n" +
                                                        $"Error: {string.Join("\n", deleteResult.Error)}");
            }
        }

        await dataContext.Timeslots.Where(t => t.Id == timeslotId).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}