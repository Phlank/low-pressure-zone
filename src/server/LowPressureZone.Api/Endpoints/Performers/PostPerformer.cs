using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PostPerformer(DataContext dataContext) : EndpointWithMapper<PerformerRequest, PerformerMapper>
{
    public override void Configure()
    {
        Post("/performers");
        Description(b => b.Produces(201));
        Roles(RoleNames.All.ToArray());
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var performer = await Map.ToEntityAsync(req, ct);
        dataContext.Performers.Add(performer);
        await dataContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetPerformerById>(new { performer.Id }, Response, cancellation: ct);
    }
}
