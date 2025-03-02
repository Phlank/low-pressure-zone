using FastEndpoints;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudiences : EndpointWithoutRequest<IEnumerable<AudienceResponse>, AudienceMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/audiences");
        Description(b => b.Produces<List<AudienceResponse>>(200));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var audiences = DataContext.Audiences.AsNoTracking().ToList();
        var responses = audiences.Select(Map.FromEntity).AsEnumerable();
        await SendOkAsync(responses, ct);
    }
}
