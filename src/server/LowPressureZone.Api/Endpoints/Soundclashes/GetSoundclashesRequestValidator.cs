using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants.Errors;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class GetSoundclashesRequestValidator : Validator<GetSoundclashesRequest>
{
    public GetSoundclashesRequestValidator()
    {
        RuleFor(req => req.ScheduleId).NotEmpty().WithMessage(Errors.Required);
    }
}