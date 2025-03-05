using FastEndpoints;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class PostAudience(DataContext dataContext) : EndpointWithMapper<AudienceRequest, AudienceMapper>
{
    public override void Configure()
    {
        Post("/audiences");
        Description(b => b.Produces(201));
        AllowAnonymous();
    }

    public override async Task HandleAsync(AudienceRequest req, CancellationToken ct)
    {
        var audience = await Map.ToEntityAsync(req, ct);
        dataContext.Audiences.Add(audience);
        await dataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetAudiences>(new { audience.Id }, Response, cancellation: ct);
    }
}
