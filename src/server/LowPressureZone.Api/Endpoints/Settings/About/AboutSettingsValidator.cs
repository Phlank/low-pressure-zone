using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;

namespace LowPressureZone.Api.Endpoints.Settings.About;

public class AboutSettingsValidator : Validator<AboutSettingsRequest>
{
    public AboutSettingsValidator()
    {
        RuleFor(req => req.TopText).NotEmpty().WithMessage(Errors.Required);
        RuleFor(req => req.BottomText).NotEmpty().WithMessage(Errors.Required);
    }
}