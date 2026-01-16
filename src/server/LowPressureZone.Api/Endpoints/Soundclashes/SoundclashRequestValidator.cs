using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class SoundclashRequestValidator : Validator<SoundclashRequest>
{
    public SoundclashRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(req => req.ScheduleId).NotEqual(Guid.Empty).WithMessage(Errors.Required);
        RuleFor(req => req.PerformerOneId).NotEqual(Guid.Empty).WithMessage(Errors.Required);
        RuleFor(req => req.PerformerTwoId).NotEqual(Guid.Empty).WithMessage(Errors.Required);
        RuleFor(req => req.RoundOne).NotEmpty().WithMessage(Errors.Required)
                                    .MaximumLength(32).WithMessage(Errors.MaxLength(32));
        RuleFor(req => req.RoundTwo).NotEmpty().WithMessage(Errors.Required)
                                    .MaximumLength(32).WithMessage(Errors.MaxLength(32));
        RuleFor(req => req.RoundThree).NotEmpty().WithMessage(Errors.Required)
                                      .MaximumLength(32).WithMessage(Errors.MaxLength(32));
        RuleFor(req => req.StartsAt).LessThan(req => req.EndsAt);
        RuleFor(req => req.EndsAt).Equal(req => req.StartsAt.AddHours(2)).WithMessage("Soundclash must be two hours");

        RuleFor(req => req).CustomAsync(async (req, context, ct) =>
        {
            var id = contextAccessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();
            var schedule = await dataContext.Schedules
                                            .Include(schedule => schedule.Soundclashes)
                                            .Where(schedule => schedule.Id == req.ScheduleId)
                                            .FirstOrDefaultAsync(ct);
            if (schedule is null)
            {
                context.AddFailure(nameof(req.ScheduleId), Errors.DoesNotExist);
                return;
            }

            Guid[] performerIds = [req.PerformerOneId, req.PerformerTwoId];
            var performers = await dataContext.Performers
                                              .Where(performer => performerIds.Contains(performer.Id))
                                              .ToDictionaryAsync(performer => performer.Id, performer => performer, ct);

            if (!performers.ContainsKey(req.PerformerOneId))
            {
                context.AddFailure(nameof(req.PerformerOneId), Errors.DoesNotExist);
                return;
            }

            if (!performers.ContainsKey(req.PerformerTwoId))
            {
                context.AddFailure(nameof(req.PerformerTwoId), Errors.DoesNotExist);
                return;
            }

            if (schedule.Type != ScheduleType.Soundclash)
                context.AddFailure(nameof(req.ScheduleId), SoundclashErrors.ScheduleNotCorrectType);
            
            if (req.StartsAt < schedule.StartsAt || req.StartsAt > schedule.EndsAt)
                context.AddFailure(nameof(req.StartsAt), Errors.OutOfScheduleRange);
            if (req.EndsAt < schedule.StartsAt || req.EndsAt > schedule.EndsAt)
                context.AddFailure(nameof(req.EndsAt), Errors.OutOfScheduleRange);
            if (schedule.Soundclashes.WhereOverlaps(req).Any(soundclash => soundclash.Id != id))
            {
                context.AddFailure(nameof(req.StartsAt), Errors.OverlapsOtherTimeslot);
                context.AddFailure(nameof(req.EndsAt), Errors.OverlapsOtherTimeslot);
            }
        });
    }
}