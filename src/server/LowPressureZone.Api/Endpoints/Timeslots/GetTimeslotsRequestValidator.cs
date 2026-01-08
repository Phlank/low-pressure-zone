using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public class GetTimeslotsRequestValidator : Validator<GetTimeslotsRequest>
{
    public GetTimeslotsRequestValidator()
    {
        RuleFor(req => req.ScheduleId).NotEmpty().WithMessage(Errors.Required);
    }
}