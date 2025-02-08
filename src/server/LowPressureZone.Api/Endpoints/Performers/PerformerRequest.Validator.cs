using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestValidator : Validator<PerformerRequest>
{
    public PerformerRequestValidator()
    {
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Url).NotEmpty().AbsoluteHttpUri();
    }
}
