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

public class PostTimeslot : Endpoint<TimeslotRequest, EmptyResponse, TimeslotRequestMapper>
{
    private readonly DataContext _dataContext;
    private readonly TimeslotRules _timeslotEnforcer;
    private readonly PerformerRules _performerEnforcer;

    public PostTimeslot(DataContext dataContext, TimeslotRules enforcer, PerformerRules performerEnforcer)
    {
        _dataContext = dataContext;
        _timeslotEnforcer = enforcer;
        _performerEnforcer = performerEnforcer;
    }

    public override void Configure()
    {
        Post("/schedules/{scheduleId}/timeslots");
        Description(b => b.Produces(201)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(TimeslotRequest req, CancellationToken ct)
    {
        var scheduleId = Route<Guid>("scheduleId");
        var schedule = await _dataContext.Schedules.AsNoTracking()
                                                   .Include(nameof(Schedule.Timeslots))
                                                   .Where(s => s.Id == scheduleId)
                                                   .FirstOrDefaultAsync(ct);

        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var requestPerformer = await _dataContext.Performers.AsNoTracking()
                                                            .Where(p => p.Id == req.PerformerId)
                                                            .FirstOrDefaultAsync(ct);

        AddDataValidationErrors(req, schedule, requestPerformer);
        AddBusinessRuleErrors(requestPerformer);
        ThrowIfAnyErrors();

        var entity = Map.ToEntity(req);
        entity.ScheduleId = scheduleId;
        _dataContext.Timeslots.Add(entity);
        _dataContext.SaveChanges();
        await SendCreatedAtAsync<GetScheduleById>(new { id = scheduleId }, Response);
    }

    private void AddDataValidationErrors(TimeslotRequest req, Schedule schedule, Performer? requestPerformer)
    {
        if (req.Start < schedule.Start || req.Start > schedule.End)
        {
            AddError(new ValidationFailure(nameof(req.Start), Errors.OutOfScheduleRange));
        }
        if (req.End < schedule.Start || req.End > schedule.End)
        {
            AddError(new ValidationFailure(nameof(req.End), Errors.OutOfScheduleRange));
        }

        var doesOverlapOtherTimeslot = schedule.Timeslots.WhereOverlaps(req).Any();
        if (doesOverlapOtherTimeslot)
        {
            AddError(new ValidationFailure(nameof(req.Start), Errors.OverlapsOtherTimeslot));
            AddError(new ValidationFailure(nameof(req.End), Errors.OverlapsOtherTimeslot));
        }

        if (requestPerformer == null)
        {
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.DoesNotExist));
        }
    }

    private void AddBusinessRuleErrors(Performer? requestPerformer)
    {
        if (requestPerformer != null && _performerEnforcer.CanUserLinkPerformer(requestPerformer))
        {
            AddError(new ValidationFailure(nameof(TimeslotRequest.PerformerId), Errors.EntityNotLinked));
        }
    }
}
