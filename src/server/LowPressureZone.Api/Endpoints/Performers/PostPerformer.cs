using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PostPerformer(DataContext dataContext) : EndpointWithMapper<PerformerRequest, PerformerMapper>
{
    public override void Configure()
    {
        Post("/performers");
        Description(builder => builder.Produces(201));
    }

    public override async Task HandleAsync(PerformerRequest request, CancellationToken ct)
    {
        var performer = Map.ToEntity(request);
        dataContext.Performers.Add(performer);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await SendCreatedAtAsync<GetPerformerById>(new
        {
            performer.Id
        }, Response, cancellation: ct);
    }
}
