using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Data;
using LowPressureZone.Domain.PerformerAggregate;
using LowPressureZone.Identity.Extensions;

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
        var result = Performer.Create(User.GetIdOrDefault(), request.Name, request.SocialUrl);
        await this.PublishOrThrowAsync(result);
        
        dataContext.Performers.Add(result.Value);
        await dataContext.SaveChangesAsync(ct);
        HttpContext.ExposeLocation();
        await Send.CreatedAtAsync<GetPerformerById>(new
        {
            result.Value.Id
        }, Response, cancellation: ct);
    }
}