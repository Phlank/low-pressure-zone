using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Endpoints.Users.Streamers;

public class StreamerRequestValidator : Validator<StreamerRequest>
{
    public StreamerRequestValidator()
    {
        RuleFor(request => request.DisplayName).NotEmpty()
                                               .WithMessage(Errors.Required)
                                               .MaximumLength(128)
                                               .WithMessage(Errors.MaxLength(128));
    }
}