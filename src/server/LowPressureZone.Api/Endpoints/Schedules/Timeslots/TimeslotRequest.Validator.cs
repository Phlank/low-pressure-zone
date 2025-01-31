using FastEndpoints;
using FluentValidation;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestValidator : Validator<TimeslotRequest>
{
    public TimeslotRequestValidator()
    {
        RuleFor(t => t.PerformanceType).NotEqual(PerformanceType.None);
        RuleFor(t => t.Start).LessThan(t  => t.End);
        RuleFor(t => t.End).GreaterThan(t => t.Start);
    }
}
