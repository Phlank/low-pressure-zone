using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants.Errors;

namespace LowPressureZone.Api.Endpoints.News;

public class NewsRequestValidator : Validator<NewsRequest>
{
    public NewsRequestValidator()
    {
        RuleFor(request => request.Title).NotEmpty().WithMessage(Errors.Required);
        RuleFor(request => request.Body).NotEmpty().WithMessage(Errors.Required);
    }
}