using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestValidator : Validator<TimeslotRequest>
{
    public TimeslotRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(t => t.PerformanceType).Must(t => PerformanceTypes.All.Contains(t))
                                       .WithMessage("Invalid type");
        RuleFor(t => t.StartsAt).GreaterThan(DateTime.UtcNow).WithMessage(Errors.TimeInPast);
        RuleFor(t => t.EndsAt).GreaterThan(t => t.StartsAt);

        RuleFor(t => t).CustomAsync(async (req, ctx, ct) =>
        {
            var scheduleId = contextAccessor.GetGuidRouteParameterOrDefault("scheduleId");
            var timeslotId = contextAccessor.GetGuidRouteParameterOrDefault("timeslotId");
            var dataContext = Resolve<DataContext>();
            var schedule = await dataContext.Schedules.Include(s => s.Timeslots)
                                            .Where(s => s.Id == scheduleId)
                                            .FirstOrDefaultAsync(ct);
            var performer = await dataContext.Performers.Where(p => p.Id == req.PerformerId)
                                             .FirstOrDefaultAsync(ct);

            if (performer is null)
                ctx.AddFailure(nameof(req.PerformerId), Errors.DoesNotExist);
            if (schedule is null)
            {
                ctx.AddFailure("Schedule does not exist");
                return;
            }

            if (req.StartsAt < schedule.StartsAt || req.EndsAt > schedule.EndsAt)
                ctx.AddFailure(nameof(req.StartsAt), Errors.OutOfScheduleRange);
            if (req.EndsAt < schedule.StartsAt || req.EndsAt > schedule.EndsAt)
                ctx.AddFailure(nameof(req.EndsAt), Errors.OutOfScheduleRange);
            if (schedule.Timeslots.WhereOverlaps(req).Any(t => t.Id != timeslotId))
            {
                ctx.AddFailure(nameof(req.StartsAt), Errors.OverlapsOtherTimeslot);
                ctx.AddFailure(nameof(req.EndsAt), Errors.OverlapsOtherTimeslot);
            }
        });
    }
}
