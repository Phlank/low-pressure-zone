using FastEndpoints;
using FFMpegCore;
using FluentValidation;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services.Audio;
using LowPressureZone.Api.Utilities;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public sealed class TimeslotRequestValidator : Validator<TimeslotRequest>
{
    private const int PrerecordedDurationMinutesTolerance = 2;

    public TimeslotRequestValidator(IHttpContextAccessor contextAccessor, ILogger<TimeslotRequestValidator> logger)
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

            if (request.PerformanceType == PerformanceTypes.Prerecorded
                && timeslotId == Guid.Empty
                && request.File is null)
            {
                context.AddFailure(nameof(request.File), Errors.Required);
                return;
            }

            if (request.PerformanceType == PerformanceTypes.Prerecorded
                && request.ReplaceMedia == true
                && request.File is null)
            {
                context.AddFailure(nameof(request.File), Errors.Required);
                return;
            }

            if (request.PerformanceType == PerformanceTypes.Prerecorded
                && request.ReplaceMedia == false
                && request.File is not null)
            {
                context.AddFailure(nameof(request.File), Errors.Prohibited);
                return;
            }

            if (request.PerformanceType != PerformanceTypes.Prerecorded
                && request.File is not null)
            {
                context.AddFailure(nameof(request.File), Errors.Prohibited);
                return;
            }

            if (request.File is not null
                && !request.File.ContentType.StartsWithAny(MimeTypes.AudioMimeTypes))
            {
                context.AddFailure(nameof(request.File), Errors.InvalidFileType);
                return;
            }

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

            if (request.StartsAt < schedule.StartsAt || request.StartsAt > schedule.EndsAt)
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

    public static ICollection<ValidationFailure> ValidateMediaAnalysis(TimeslotRequest request, IMediaAnalysis analysis)
    {
        request.File.ShouldNotBeNull();
        List<ValidationFailure> failures = [];
        var timeslotDuration = request.EndsAt - request.StartsAt;
        if (TimeSpan.FromMinutes(timeslotDuration.TotalMinutes - PrerecordedDurationMinutesTolerance) > analysis.Duration
            || TimeSpan.FromMinutes(timeslotDuration.TotalMinutes + PrerecordedDurationMinutesTolerance) < analysis.Duration)
        {
            failures.Add(new ValidationFailure(nameof(request.File),
                                               "Media file duration does not match the specified timeslot duration. Ensure it is +/- 2 minutes from the timeslot duration."));
        }

        failures.AddRange(AudioQualityValidator.ValidateAudioQuality(analysis, request.File.Length, nameof(request.File)));
        return failures;
    }
}