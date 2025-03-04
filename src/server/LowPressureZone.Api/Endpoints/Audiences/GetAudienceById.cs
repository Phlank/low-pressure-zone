using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudienceById(DataContext dataContext) : EndpointWithoutRequest<AudienceResponse, AudienceMapper>
{
    public override void Configure()
    {
        Get("/audiences/{id}");
        Description(b => b.Produces<AudienceResponse>(200)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var audience = await dataContext.Audiences.AsNoTracking().FirstOrDefaultAsync(aud => aud.Id == id && !aud.IsDeleted, ct);
        if (audience == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = await Map.FromEntityAsync(audience, ct);
        await SendOkAsync(response, ct);
    }
}
