using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public sealed class CommunityRequestValidator : Validator<CommunityRequest>
{
    public CommunityRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(request => request.Name).NotEmpty()
                                        .WithMessage(Errors.Required);
        RuleFor(request => request.Url).NotEmpty()
                                       .AbsoluteHttpUri();

        RuleFor(request => request).CustomAsync(async (request, validationContext, ct) =>
        {
            var id = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Communities.AnyAsync(community => community.Name == request.Name && community.Id != id, ct);
            if (isNameInUse)
                validationContext.AddFailure(nameof(request.Name), Errors.Unique);
        });
    }
}
