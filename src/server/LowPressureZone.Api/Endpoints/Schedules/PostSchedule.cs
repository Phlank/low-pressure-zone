using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PostSchedule : EndpointWithMapper<ScheduleRequest, ScheduleMapper>
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
            AddError(new ValidationFailure(nameof(req.StartsAt), "Overlaps other schedule"));
            AddError(new ValidationFailure(nameof(req.EndsAt), "Overlaps other schedule"));
        }
        var doesCommunityExist = DataContext.Communities.Any(a => a.Id == req.CommunityId);
        if (!doesCommunityExist)
        {
            AddError(new ValidationFailure(nameof(req.CommunityId), "Invalid community"));
        }
        ThrowIfAnyErrors();

        DataContext.Schedules.Add(schedule);
        await DataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetScheduleById>(new { schedule.Id }, Response, cancellation: ct);
    }
}
