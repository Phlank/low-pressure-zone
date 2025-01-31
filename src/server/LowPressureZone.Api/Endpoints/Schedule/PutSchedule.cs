using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class PutSchedule : Endpoint<ScheduleRequest, EmptyResponse, ScheduleRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/schedules/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
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

        var isChanged = false;
        if (req.Start != schedule.Start || req.End != schedule.End)
        {
            isChanged = true;
            var doesOverlapAnySchedule = DataContext.Schedules.Where(s => s.Id != id)
                                                              .WhereOverlaps(req)
                                                              .Any();
            if (doesOverlapAnySchedule)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Schedule times cannot overlap."));
                AddError(new ValidationFailure(nameof(req.End), "Schedule times cannot overlap."));
            }
            var doesExcludeAnyTimeslots = DataContext.Timeslots.Where(t => t.ScheduleId == id)
                                                               .WhereNotInside(req)
                                                               .Any();
            if (doesExcludeAnyTimeslots)
            {
                AddError(new ValidationFailure(nameof(req.Start), "Time range cannot exclude linked timeslots."));
                AddError(new ValidationFailure(nameof(req.End), "Time range cannot exclude linked timeslots."));
            }
        }

        if (req.AudienceId != schedule.AudienceId)
        {
            isChanged = true;
            if (DataContext.Audiences.Any(a => a.Id == id))
            {
                AddError(new ValidationFailure(nameof(req.AudienceId), "Invalid audience specified."));
            }
        }

        ThrowIfAnyErrors();
        
        if (isChanged)
        {
            schedule.Start = req.Start;
            schedule.End = req.End;
            schedule.AudienceId = req.AudienceId;
            schedule.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }
        await SendNoContentAsync(ct);
    }
}
