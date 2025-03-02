using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceRequestValidator : Validator<AudienceRequest>
{
    public AudienceRequestValidator(AudienceRules rules)
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Url).NotEmpty().AbsoluteHttpUri();
    }
}
