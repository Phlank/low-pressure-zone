using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
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
        var schedule = await DataContext.Schedules.AsNoTracking()
                                                  .Include(nameof(Schedule.Timeslots))
                                                  .Where(s => s.Id == scheduleId)
                                                  .FirstOrDefaultAsync(ct);

        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var requestPerformer = await DataContext.Performers.AsNoTracking()
                                                           .Where(p => p.Id == req.PerformerId)
                                                           .FirstOrDefaultAsync(ct);

        AddDataValidationErrors(req, schedule, requestPerformer);
        AddBusinessRuleErrors(req, schedule, requestPerformer);
        ThrowIfAnyErrors();

        var entity = Map.ToEntity(req);
        entity.ScheduleId = scheduleId;
        DataContext.Timeslots.Add(entity);
        DataContext.SaveChanges();
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
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.InvalidPerformer));
        }
    }

    private void AddBusinessRuleErrors(TimeslotRequest req, Schedule schedule, Performer? requestPerformer)
    {
        if (requestPerformer != null && !PerformerRules.CanUserLinkPerformer(User, requestPerformer))
        {
            AddError(new ValidationFailure(nameof(req.PerformerId), Errors.EntityNotLinked));
        }
    }
}
