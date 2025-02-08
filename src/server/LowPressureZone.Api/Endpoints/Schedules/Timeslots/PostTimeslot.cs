using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Domain.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PostTimeslot : Endpoint<TimeslotRequest, EmptyResponse, TimeslotRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/schedules/{scheduleId}/timeslots");
        Description(b => b.Produces(201)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var schedule = DataContext.Schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var isInsideSchedule = req.IsWithin(schedule);
        if (!isInsideSchedule)
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

        if (!DataContext.Has<Performer>(req.PerformerId))
        {
            AddError(nameof(req.PerformerId), "Invalid performer specified.");
        }

        ThrowIfAnyErrors();


        DataContext.Timeslots.Add(Map.ToEntity(req));
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetScheduleById>(new { id = scheduleId }, Response);
    }
}
