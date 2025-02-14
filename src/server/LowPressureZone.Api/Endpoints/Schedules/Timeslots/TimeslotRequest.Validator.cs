using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestValidator : Validator<TimeslotRequest>
{
    public TimeslotRequestValidator()
    {
        RuleFor(t => t.PerformanceType).Must(t => PerformanceTypes.All.Contains(t)).WithMessage("Invalid type");
        RuleFor(t => t.Start).LessThan(t  => t.End);
        RuleFor(t => t.End).GreaterThan(t => t.Start);
    }
}
