using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestValidator : Validator<TimeslotRequest>
{
    public TimeslotRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(request => request.PerformanceType).Must(type => PerformanceTypes.All.Contains(type))
                                                   .WithMessage("Invalid type");
        RuleFor(request => request.StartsAt).GreaterThan(DateTime.UtcNow).WithMessage(Errors.TimeInPast);
        RuleFor(request => request.EndsAt).GreaterThan(request => request.StartsAt);

        RuleFor(request => request.Name).MaximumLength(64)
                                        .WithMessage(Errors.MaxLength(64));

        RuleFor(t => t).CustomAsync(async (request, context, ct) =>
        {
            var scheduleId = contextAccessor.GetGuidRouteParameterOrDefault("scheduleId");
            var timeslotId = contextAccessor.GetGuidRouteParameterOrDefault("timeslotId");
            var dataContext = Resolve<DataContext>();
            var schedule = await dataContext.Schedules
                                            .Include(schedule => schedule.Timeslots)
                                            .Where(schedule => schedule.Id == scheduleId)
                                            .FirstOrDefaultAsync(ct);
            var performer = await dataContext.Performers
                                             .Where(performer => performer.Id == request.PerformerId)
                                             .FirstOrDefaultAsync(ct);

            if (performer is null)
                context.AddFailure(nameof(request.PerformerId), Errors.DoesNotExist);
            if (schedule is null)
            {
                context.AddFailure(nameof(scheduleId), Errors.DoesNotExist);
                return;
            }

            if (request.StartsAt < schedule.StartsAt || request.EndsAt > schedule.EndsAt)
                context.AddFailure(nameof(request.StartsAt), Errors.OutOfScheduleRange);
            if (request.EndsAt < schedule.StartsAt || request.EndsAt > schedule.EndsAt)
                context.AddFailure(nameof(request.EndsAt), Errors.OutOfScheduleRange);
            if (schedule.Timeslots.WhereOverlaps(request).Any(timeslot => timeslot.Id != timeslotId))
            {
                context.AddFailure(nameof(request.StartsAt), Errors.OverlapsOtherTimeslot);
                context.AddFailure(nameof(request.EndsAt), Errors.OverlapsOtherTimeslot);
            }
        });
    }
}