using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Routing;

namespace LowPressureZone.Api.Endpoints.Audience;

public class PutAudienceValidator : Validator<PutAudienceRequest>
{
    public PutAudienceValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Url).NotEmpty().Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Invalid URL.");
    }
}
