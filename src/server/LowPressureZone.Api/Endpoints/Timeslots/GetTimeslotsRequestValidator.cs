using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public class GetTimeslotsRequestValidator : Validator<GetTimeslotsRequest>
{
    public GetTimeslotsRequestValidator()
    {
        RuleFor(req => req.ScheduleId).NotEmpty().WithMessage(Errors.Required);
    }
}