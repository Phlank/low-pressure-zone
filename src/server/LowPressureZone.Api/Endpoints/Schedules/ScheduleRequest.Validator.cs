using FastEndpoints;
using FluentValidation;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleRequestValidator : Validator<ScheduleRequest>
{
    public ScheduleRequestValidator()
    {
        RuleFor(s => s.End).GreaterThan(DateTime.UtcNow)
                           .WithMessage("Cannot update schedules in the past")
                           .GreaterThan(s => s.Start)
                           .WithMessage("Less than start");
    }
}
