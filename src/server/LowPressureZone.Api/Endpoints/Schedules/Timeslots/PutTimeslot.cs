using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
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

        var timeslot = await DataContext.Timeslots.AsNoTracking()
                                                  .Include(nameof(Timeslot.Schedule))
                                                  .Include($"{nameof(Timeslot.Schedule)}.{nameof(Timeslot.Schedule.Timeslots)}")
                                                  .Include(nameof(Timeslot.Performer))
                                                  .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                                  .AsSplitQuery()
                                                  .FirstOrDefaultAsync(ct);
        
        if (timeslot == null || timeslot.Schedule == null)
        {
            await SendNotFoundAsync();
            return;
        }

        Performer? requestPerformer = await DataContext.Performers.AsNoTracking()
                                                                  .Where(p => p.Id == req.PerformerId)
                                                                  .FirstOrDefaultAsync(ct);

        AddDataValidationErrors(req, timeslot, requestPerformer);
        AddBusinessRuleErrors(req, timeslot, requestPerformer);
        ThrowIfAnyErrors();

        var trackedTimeslot = await DataContext.Timeslots.Where(t => t.Id == timeslotId)
                                                         .FirstOrDefaultAsync(ct);
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

    private void AddDataValidationErrors(TimeslotRequest req, Timeslot timeslot, Performer? requestPerformer)
    {
        if (req.Start != timeslot.Start || req.End != timeslot.End)
        {
            if (req.Start < timeslot.Schedule!.Start || req.Start > timeslot.Schedule.End)
            {
                AddError(new ValidationFailure(nameof(req.Start), Errors.OutOfScheduleRange));
            }
            if (req.End < timeslot.Schedule.Start || req.End > timeslot.Schedule.End)
            {
                AddError(new ValidationFailure(nameof(req.End), Errors.OutOfScheduleRange));
            }

            var doesOverlapOtherTimeslot = timeslot.Schedule.Timeslots.WhereOverlaps(req).Any();
            if (doesOverlapOtherTimeslot)
            {
                AddError(new ValidationFailure(nameof(req.Start), Errors.OverlapsOtherTimeslot));
                AddError(new ValidationFailure(nameof(req.End), Errors.OverlapsOtherTimeslot));
            }
        }
        
        if (requestPerformer == null)
        {
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.InvalidPerformer));
        }
    }

    private void AddBusinessRuleErrors(TimeslotRequest req, Timeslot timeslot, Performer? requestPerformer)
    {
        if (!TimeslotRules.CanUserEditTimeslot(User, timeslot))
        {
            AddError(Errors.TimeslotNotEditable);
        }

        if (requestPerformer != null && !PerformerRules.CanUserLinkPerformer(User, requestPerformer))
        {
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.EntityNotLinked));
        }
    }
}
