using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants.Errors;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestValidator : Validator<PerformerRequest>
{
    public PerformerRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(request => request.Name).NotEmpty()
                                        .WithMessage(Errors.Required)
                                        .MaximumLength(64)
                                        .WithMessage(Errors.MaxLength(64));

        When(request => !string.IsNullOrEmpty(request.SocialUrl), () =>
        {
            RuleFor(request => request.SocialUrl!).MaximumLength(64)
                                            .WithMessage(Errors.MaxLength(256))
                                            .AbsoluteHttpUri();
        });
    }
}