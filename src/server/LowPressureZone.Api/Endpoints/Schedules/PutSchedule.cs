using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PutSchedule : EndpointWithMapper<ScheduleRequest, ScheduleRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/schedules/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(ScheduleRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var schedule = DataContext.Schedules.Find(id);
        if (schedule == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (req.Start != schedule.Start || req.End != schedule.End)
        {
            var doesOverlapAnySchedule = DataContext.Schedules.Where(s => s.Id != id).WhereOverlaps(req).Any();
            if (doesOverlapAnySchedule)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Overlaps other schedule"));
                AddError(new ValidationFailure(nameof(req.End), "Overlaps other schedule"));
            }
            var doesExcludeAnyTimeslots = DataContext.Timeslots.Where(t => t.ScheduleId == id).WhereNotInside(req).Any();
            if (doesExcludeAnyTimeslots)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Excludes timeslots"));
                AddError(new ValidationFailure(nameof(req.End), "Excludes timeslots"));
            }
        }

        if (req.AudienceId != schedule.AudienceId)
        {
            if (!DataContext.Has<Audience>(req.AudienceId))
            {
                AddError(new ValidationFailure(nameof(req.AudienceId), "Invalid audience"));
            }
        }

        ThrowIfAnyErrors();

        schedule.Start = req.Start;
        schedule.End = req.End;
        schedule.AudienceId = req.AudienceId;
        if (DataContext.ChangeTracker.HasChanges())
        {
            schedule.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendNoContentAsync(ct);
    }
}
