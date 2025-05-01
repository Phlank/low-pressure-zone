using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestValidator : Validator<PerformerRequest>
{
    public PerformerRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(request => request.Name).NotEmpty()
                                        .WithMessage(Errors.Required)
                                        .MaximumLength(64)
                                        .WithMessage(Errors.MaxLength(64));

        When(request => !string.IsNullOrEmpty(request.Url), () =>
        {
            RuleFor(request => request.Url!).MaximumLength(64)
                                            .WithMessage(Errors.MaxLength(256))
                                            .AbsoluteHttpUri();
        });
    }
}
