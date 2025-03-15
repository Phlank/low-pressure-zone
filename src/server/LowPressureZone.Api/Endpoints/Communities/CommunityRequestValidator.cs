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
        RuleFor(request => request.Url).NotEmpty().AbsoluteHttpUri();
        
        RuleFor(request => request).CustomAsync(async (req, ctx, ct) =>
        {
            var id = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Communities.AnyAsync(a => a.Name == req.Name && a.Id != id, ct);
            if (isNameInUse)
                ctx.AddFailure(nameof(req.Name), Errors.Unique);
        });
    }
}
