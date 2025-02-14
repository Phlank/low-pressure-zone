using FastEndpoints;
using FluentValidation.Results;
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
        AllowAnonymous();
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = DataContext.Timeslots.Include("Schedule").FirstOrDefault(t => t.Id == timeslotId && t.ScheduleId == scheduleId);
        if (timeslot == null || timeslot.Schedule == null)
        {
            await SendNotFoundAsync();
            return;
        }

        if (req.Start != timeslot.Start || req.End != timeslot.End)
        {
            if (req.Start < timeslot.Schedule.Start || req.Start > timeslot.Schedule.End)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Exceeds schedule"));
            }
            if (req.End < timeslot.Schedule.Start || req.End > timeslot.Schedule.End)
            {
                AddError(new ValidationFailure(nameof(req.End), "Exceeds schedule"));
            }

            var doesOverlapOtherTimeslot = DataContext.Timeslots.Where(t => t.ScheduleId == scheduleId).WhereOverlaps(req).Any();
            if (doesOverlapOtherTimeslot)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Overlaps other timeslot"));
                AddError(new ValidationFailure(nameof(req.End), "Overlaps other timeslot"));
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
        timeslot.Name = req.Name;
        if (DataContext.ChangeTracker.HasChanges())
        {
            timeslot.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendNoContentAsync(ct);
    }
}
