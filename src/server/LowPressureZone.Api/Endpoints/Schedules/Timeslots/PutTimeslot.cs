using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class PutTimeslot : EndpointWithMapper<TimeslotRequest, TimeslotRequestMapper>
{
    private readonly DataContext _dataContext;
    private readonly TimeslotRules _rules;

    public PutTimeslot(DataContext dataContext, TimeslotRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Put("/schedules/{scheduleId}/timeslots/{timeslotId}");
        Description(b => b.Produces(204)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var timeslotId = Route<Guid>("timeslotId");

        var timeslot = await _dataContext.Timeslots.AsNoTracking()
                                                  .AsSplitQuery()
                                                  .Include(nameof(Timeslot.Schedule))
                                                  .Include($"{nameof(Timeslot.Schedule)}.{nameof(Schedule.Timeslots)}")
                                                  .Include(nameof(Timeslot.Performer))
                                                  .Where(t => t.Id == timeslotId && t.ScheduleId == scheduleId)
                                                  .FirstOrDefaultAsync(ct);
        
        if (timeslot == null || timeslot.Schedule == null)
        {
            await SendNotFoundAsync();
            return;
        }

        Performer? requestPerformer = await _dataContext.Performers.AsNoTracking()
                                                                  .Where(p => p.Id == req.PerformerId)
                                                                  .FirstOrDefaultAsync(ct);

        AddDataValidationErrors(req, timeslot, requestPerformer);
        AddBusinessRuleErrors(timeslot, requestPerformer);
        ThrowIfAnyErrors();

        var trackedTimeslot = await _dataContext.Timeslots.Where(t => t.Id == timeslotId)
                                                         .FirstAsync(ct);
        trackedTimeslot.Start = req.Start;
        trackedTimeslot.End = req.End;
        trackedTimeslot.PerformerId = req.PerformerId;
        trackedTimeslot.Type = req.PerformanceType;
        trackedTimeslot.Name = req.Name;
        if (_dataContext.ChangeTracker.HasChanges())
        {
            trackedTimeslot.LastModifiedDate = DateTime.UtcNow;
            _dataContext.SaveChanges();
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
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.DoesNotExist));
        }
    }

    private void AddBusinessRuleErrors(Timeslot timeslot, Performer? requestPerformer)
    {
        if (_rules.CanUserEditTimeslot(timeslot))
        {
            AddError(Errors.TimeslotNotEditable);
        }

        if (requestPerformer != null && !_rules.CanUserLinkPerformerToTimeslot(requestPerformer)) {
            AddError(new ValidationFailure(nameof(TimeslotRequest.PerformerId), Errors.EntityNotLinked));
        }
    }
}
