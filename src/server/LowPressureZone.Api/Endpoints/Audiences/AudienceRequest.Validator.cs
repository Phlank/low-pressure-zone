using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceRequestValidator : Validator<AudienceRequest>
{
    public AudienceRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Url).NotEmpty().AbsoluteHttpsUri();
    }
}
