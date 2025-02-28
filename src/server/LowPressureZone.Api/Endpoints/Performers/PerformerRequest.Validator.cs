using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestValidator : Validator<PerformerRequest>
{
    public PerformerRequestValidator(IHttpContextAccessor accessor, PerformerRules rules)
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage(Errors.Required).Must(name =>
        {
            var performerId = accessor.GetGuidRouteParameterOrDefault("id");
            return !rules.IsNameInUse(name, performerId);
        }).WithMessage(Errors.Unique);
        RuleFor(p => p.Url).NotEmpty().AbsoluteHttpUri();
    }
}
