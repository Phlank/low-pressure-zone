using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PostSchedule : EndpointWithMapper<ScheduleRequest, ScheduleRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/schedules");
        Description(b => b.Produces(201));
        Roles(RoleNames.Admin, RoleNames.Organizer);
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var schedule = Map.ToEntity(req);
        var doesOverlapOtherSchedule = DataContext.Schedules.WhereOverlaps(schedule).Any();
        if (doesOverlapOtherSchedule)
        {
            AddError(new ValidationFailure(nameof(req.Start), "Overlaps other schedule"));
            AddError(new ValidationFailure(nameof(req.End), "Overlaps other schedule"));
        }
        var doesAudienceExist = DataContext.Audiences.Any(a => a.Id == req.AudienceId);
        if (!doesAudienceExist)
        {
            AddError(new ValidationFailure(nameof(req.AudienceId), "Invalid audience"));
        }
        ThrowIfAnyErrors();

        DataContext.Schedules.Add(schedule);
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetScheduleById>(new { schedule.Id }, Response, cancellation: ct);
    }
}
