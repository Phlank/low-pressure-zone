using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PostSchedule : Endpoint<ScheduleRequest, EmptyResponse, ScheduleRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/schedules");
        Description(b => b.Produces(201));
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var schedule = Map.ToEntity(req);
        var doesOverlapOtherSchedule = DataContext.Schedules.WhereOverlaps(schedule).Any();
        if (doesOverlapOtherSchedule)
        {
            AddError(new ValidationFailure(nameof(req.Start), "Schedule times cannot overlap."));
            AddError(new ValidationFailure(nameof(req.End), "Schedule times cannot overlap."));
        }
        var doesAudienceExist = DataContext.Audiences.Any(a => a.Id == req.AudienceId);
        if (!doesAudienceExist)
        {
            AddError(new ValidationFailure(nameof(req.AudienceId), "Invalid audience specified."));
        }
        ThrowIfAnyErrors();

        DataContext.Schedules.Add(schedule);
        await SendCreatedAtAsync<GetScheduleById>(new { schedule.Id }, Response, cancellation: ct);
    }
}
