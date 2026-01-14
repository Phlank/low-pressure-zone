using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleRequestValidator : Validator<ScheduleRequest>
{
    public ScheduleRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(request => request.EndsAt).GreaterThanOrEqualTo(request => request.StartsAt.AddHours(1))
                                          .WithMessage(Errors.MinDuration(1))
                                          .LessThanOrEqualTo(request => request.StartsAt.AddHours(24))
                                          .WithMessage(Errors.MaxDuration(24));
        RuleFor(request => request).CustomAsync(async (request, context, ct) =>
        {
            var id = contextAccessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            if (id == Guid.Empty && request.StartsAt <= DateTime.UtcNow)
                context.AddFailure(nameof(request.StartsAt), Errors.TimeInPast);

            var community = await dataContext.Communities.FirstOrDefaultAsync(a => a.Id == request.CommunityId, ct);

            var schedule = await dataContext.Schedules
                                            .AsSplitQuery()
                                            .Include(s => s.Timeslots)
                                            .Where(s => s.Id == id)
                                            .FirstOrDefaultAsync(ct);

            if (community == null) context.AddFailure(nameof(request.CommunityId), Errors.DoesNotExist);

            var isRequestOverlappingOtherSchedule = await dataContext.Schedules.WhereOverlaps(request)
                                                                     .Where(s => s.Id != id)
                                                                     .AnyAsync(ct);
            if (isRequestOverlappingOtherSchedule)
            {
                context.AddFailure(nameof(request.StartsAt), Errors.OverlapsOtherSchedule);
                context.AddFailure(nameof(request.EndsAt), Errors.OverlapsOtherSchedule);
            }

            if (schedule != null)
            {
                if (schedule.Type != request.Type)
                    context.AddFailure(nameof(request.Type), "Cannot change type");
                
                if (schedule.Timeslots.Any(t => request.StartsAt > t.StartsAt))
                    context.AddFailure(nameof(request.StartsAt), Errors.ExcludesTimeslots);

                if (schedule.Timeslots.Any(t => request.EndsAt < t.EndsAt))
                    context.AddFailure(nameof(request.EndsAt), Errors.ExcludesTimeslots);
            }
        });
    }
}