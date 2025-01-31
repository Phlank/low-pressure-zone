using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudiences : Endpoint<EmptyRequest, IEnumerable<AudienceResponse>, AudienceResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/audiences");
        Description(b => b.Produces<IEnumerable<AudienceResponse>>(200));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var audiences = DataContext.Audiences.AsNoTracking().ToList();
        await SendOkAsync(audiences.Select(Map.FromEntity), ct);
    }
}
