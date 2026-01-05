using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public partial class PostTimeslot(
    DataContext dataContext,
    PrerecordedMixFileProcessor fileProcessor,
    FormFileSaver fileSaver,
    PerformerRules performerRules,
    ScheduleRules scheduleRules)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        AllowFormData();
        AllowFileUploads();
        Post("/timeslots");
        Description(builder => builder.Produces(201));
    }

    public override async Task HandleAsync(TimeslotRequest request, CancellationToken ct)
    {
        var schedule = await dataContext.Schedules
                                        .Include(schedule => schedule.Timeslots)
                                        .Include(schedule => schedule.Community)
                                        .ThenInclude(community =>
                                                         community.Relationships.Where(relationship =>
                                                                                           relationship.UserId ==
                                                                                           User.GetIdOrDefault()))
                                        .Where(schedule => schedule.Id == request.ScheduleId)
                                        .FirstAsync(ct);
        var performer = await dataContext.Performers.FirstAsync(p => p.Id == request.PerformerId, ct);

        if (!scheduleRules.IsAddingTimeslotsAuthorized(schedule)
            || !performerRules.IsTimeslotLinkAuthorized(performer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var timeslot = Map.ToEntity(request);

        if (request.PerformanceType == PerformanceTypes.Prerecorded
            && request.File is not null)
        {
            var processResult = await fileProcessor.ProcessRequestFileToMp3Async(request, schedule.StartsAt, ct);
            if (processResult.IsError)
            {
                ValidationFailures.AddRange(processResult.Error);
                ThrowIfAnyErrors();
            }

            var uploadToAzuraCastResult = await fileProcessor.EnqueuePrerecordedMixAsync(request,
                                                                                         schedule.StartsAt,
                                                                                         processResult.Value,
                                                                                         ct);
            if (uploadToAzuraCastResult.IsError)
                ThrowError(uploadToAzuraCastResult.Error);

            timeslot.AzuraCastMediaId = uploadToAzuraCastResult.Value;

            _ = await fileSaver.DeleteFileAsync(processResult.Value);
        }

        dataContext.Timeslots.Add(timeslot);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetTimeslotById>(new
        {
            timeslot.Id
        }, Response, cancellation: ct);
    }

    [LoggerMessage(LogLevel.Error, "Unable to save file for analysis: {error}")]
    static partial void LogUnableToSaveFileError(ILogger logger, string error);
}