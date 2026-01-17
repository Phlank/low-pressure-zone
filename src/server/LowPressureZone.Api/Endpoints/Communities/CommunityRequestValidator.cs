using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Constants.Errors;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityRequestValidator : Validator<CommunityRequest>
{
    public CommunityRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(request => request.Name).NotEmpty()
                                        .WithMessage(Errors.Required)
                                        .MaximumLength(64)
                                        .WithMessage(Errors.MaxLength(64));
        RuleFor(request => request.Url).NotEmpty()
                                       .WithMessage(Errors.Required)
                                       .MaximumLength(256)
                                       .WithMessage(Errors.MaxLength(256))
                                       .AbsoluteHttpUri();

        RuleFor(request => request).CustomAsync(async (request, validationContext, ct) =>
        {
            var id = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Communities
                                               .AnyAsync(community => community.Name == request.Name
                                                                      && community.Id != id, ct);
            if (isNameInUse)
                validationContext.AddFailure(nameof(request.Name), Errors.Unique);
        });
    }
}