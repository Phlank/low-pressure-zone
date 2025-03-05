using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Constants;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PerformerRequestValidator : Validator<PerformerRequest>
{
    public PerformerRequestValidator(IHttpContextAccessor accessor)
    {
        RuleFor(p => p.Name).NotEmpty()
                            .WithMessage(Errors.Required);
        RuleFor(p => p.Url).NotEmpty()
                           .WithMessage(Errors.Required)
                           .AbsoluteHttpUri();

        RuleFor(p => p).CustomAsync(async (req, ctx, ct) =>
        {
            var performerId = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Performers.Where(p => p.Name == req.Name && p.Id != performerId)
                                                          .AnyAsync(ct);

            if (isNameInUse)
                ctx.AddFailure(nameof(req.Name), Errors.Unique);
        });
    }
}
