using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudienceById : Endpoint<EmptyRequest, AudienceResponse, AudienceResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/audiences/{id}");
        Description(b => b.Produces<AudienceResponse>(200)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var audience = DataContext.Audiences.AsNoTracking().FirstOrDefault(aud => aud.Id == id);
        if (audience == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(Map.FromEntity(audience), ct);
    }
}
