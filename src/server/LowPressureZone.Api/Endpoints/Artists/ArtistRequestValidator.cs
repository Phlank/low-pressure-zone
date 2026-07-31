using FastEndpoints;
using FluentValidation;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Artists;

public class ArtistRequestValidator : Validator<ArtistRequest>
{
    public ArtistRequestValidator(IHttpContextAccessor contextAccessor)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.").Length(1, 512);
        RuleFor(x => x.PromoUrl).AbsoluteHttpUri().Length(0, 512);

        RuleFor(x => x).CustomAsync(async (req, ctx, ct) =>
        {
            var id = contextAccessor.GetGuidRouteParameterOrDefault("id");
            var dataContext = Resolve<DataContext>();

            var matchedArtist = await dataContext.Artists
                                                 .Where(artist => artist.Name == req.Name)
                                                 .FirstOrDefaultAsync(ct);
            if (matchedArtist is not null && (id == Guid.Empty || matchedArtist.Id != id))
                ctx.AddFailure(nameof(req.Name), "Name is in use.");
        });
    }
}