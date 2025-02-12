using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleRequestValidator : Validator<ScheduleRequest>
{
    public ScheduleRequestValidator()
    {
        RuleFor(s => s.Start).GreaterThan(DateTime.UtcNow.AddDays(-1)).WithMessage("Prior to last day").LessThan(s => s.End);
        RuleFor(s => s.End).GreaterThan(DateTime.UtcNow).GreaterThan(s => s.Start);
    }
}
