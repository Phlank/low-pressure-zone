using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class AudienceRequestValidator : Validator<AudienceRequest>
{
    public AudienceRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(r => r.Name).NotEmpty()
                            .WithMessage(Errors.Required);
        RuleFor(r => r.Url).NotEmpty().AbsoluteHttpUri();

        RuleFor(r => r).CustomAsync(async (req, ctx, ct) =>
        {
            var id = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Audiences.AnyAsync(a => a.Name == req.Name && a.Id != id, ct);
            if (isNameInUse)
                ctx.AddFailure(nameof(req.Name), Errors.Unique);
        });
    }
}
