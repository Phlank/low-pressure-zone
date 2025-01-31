using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PutTimeslot : EndpointWithMapper<TimeslotRequest, TimeslotRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(b => b.Produces(204)
                          .Produces(404));
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = DataContext.Timeslots.FirstOrDefault(t => t.Id == timeslotId && t.ScheduleId == scheduleId);
        if (timeslot == null)
        {
            await SendNotFoundAsync();
            return;
        }

        if (req.Start != timeslot.Start || req.End != timeslot.End)
        {
            var isRequestWithinSchedule = DataContext.Schedules.Where(s => s.Id == scheduleId).WhereContains(req).Any();
            if (!isRequestWithinSchedule)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Timeslot range must not exceed schedule."));
                AddError(new ValidationFailure(nameof(req.End), "Timeslot range must not exceed schedule."));
            }

            var doesOverlapOtherTimeslot = DataContext.Timeslots.Where(t => t.ScheduleId == scheduleId).WhereOverlaps(req).Any();
            if (doesOverlapOtherTimeslot)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Timeslot times cannot overlap."));
                AddError(new ValidationFailure(nameof(req.End), "Timeslot times cannot overlap."));
            }
        }

        if (req.PerformerId != timeslot.PerformerId && !DataContext.Has<Performer>(req.PerformerId))
        {
            AddError(nameof(req.PerformerId), "Invalid performer specified.");
        }

        timeslot.Start = req.Start;
        timeslot.End = req.End;
        timeslot.PerformerId = req.PerformerId;
        timeslot.Type = req.PerformanceType;
        if (DataContext.ChangeTracker.HasChanges())
        {
            timeslot.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendNoContentAsync(ct);
    }
}
