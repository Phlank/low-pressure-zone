using FastEndpoints;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Api.Services;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PutTimeslot(DataContext dataContext, TimeslotFileProcessor fileProcessor, TimeslotRules rules)
    : EndpointWithMapper<TimeslotRequest, TimeslotMapper>
{
    public override void Configure()
    {
        Put("/schedules/{scheduleId}/timeslots/{timeslotId}");
        AllowFormData();
        AllowFileUploads();
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(TimeslotRequest request, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = await dataContext.Timeslots
                                        .Include(timeslot => timeslot.Performer)
                                        .Where(timeslot => timeslot.Id == timeslotId
                                                           && timeslot.ScheduleId == scheduleId)
                                        .FirstOrDefaultAsync(ct);

        if (timeslot == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!rules.IsEditAuthorized(timeslot))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        if (request.PerformanceType == PerformanceTypes.Prerecorded
            && request.File is not null)
        {
            var processResult = await fileProcessor.ProcessUploadedMediaFileAsync(request, ct);
            if (processResult.IsError)
            {
                ValidationFailures.AddRange(processResult.Error);
                ThrowIfAnyErrors();
            }
        }

        await Map.UpdateEntityAsync(request, timeslot, ct);
        await SendNoContentAsync(ct);
    }
}