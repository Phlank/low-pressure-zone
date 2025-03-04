using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleRequestValidator : Validator<ScheduleRequest>
{
    public ScheduleRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(s => s.EndsAt).GreaterThanOrEqualTo(s => s.StartsAt.AddHours(1))
                           .WithMessage(Errors.MinDuration(1))
                           .LessThanOrEqualTo(s => s.StartsAt.AddHours(24))
                           .WithMessage(Errors.MaxDuration(24));
        RuleFor(s => s).CustomAsync(async (req, ctx, ct) =>
        {
            var id = contextAccessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var audience = await dataContext.Audiences.FirstOrDefaultAsync(a => a.Id == req.AudienceId, ct);

            var schedule = await dataContext.Schedules.AsSplitQuery()
                                                      .Include(s => s.Timeslots)
                                                      .Where(s => s.Id == id)
                                                      .FirstOrDefaultAsync(ct);
            
            if (audience == null)
                ctx.AddFailure(nameof(req.AudienceId), Errors.DoesNotExist);

            var isRequestOverlappingOtherSchedule = await dataContext.Schedules.WhereOverlaps(req)
                                                                               .Where(s => s.Id != id)
                                                                               .AnyAsync(ct);
            if (isRequestOverlappingOtherSchedule)
            {
                ctx.AddFailure(nameof(req.StartsAt), Errors.OverlapsOtherSchedule);
                ctx.AddFailure(nameof(req.EndsAt), Errors.OverlapsOtherSchedule);
            }

            if (schedule != null)
            {
                if (schedule.Timeslots.Any(t => req.StartsAt > t.StartsAt))
                    ctx.AddFailure(nameof(req.StartsAt), Errors.ExcludesTimeslots);
                if (schedule.Timeslots.Any(t => req.EndsAt < t.EndsAt))
                    ctx.AddFailure(nameof(req.EndsAt), Errors.ExcludesTimeslots);
            }
        });
    }
}
