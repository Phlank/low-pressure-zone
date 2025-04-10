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
        RuleFor(request => request.Name).NotEmpty()
                                        .WithMessage(Errors.Required)
                                        .MaximumLength(64)
                                        .WithMessage(Errors.MaxLength(64));

        When(request => !string.IsNullOrEmpty(request.Url), () =>
        {
            RuleFor(request => request.Url!).MaximumLength(64)
                                            .WithMessage(Errors.MaxLength(256))
                                            .AbsoluteHttpUri();
        });

        RuleFor(request => request).CustomAsync(async (request, context, ct) =>
        {
            var performerId = accessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var isNameInUse = await dataContext.Performers
                                               .Where(performer => performer.Name == request.Name && performer.Id != performerId)
                                               .AnyAsync(ct);

            if (isNameInUse)
                context.AddFailure(nameof(request.Name), Errors.Unique);
        });
    }
}
