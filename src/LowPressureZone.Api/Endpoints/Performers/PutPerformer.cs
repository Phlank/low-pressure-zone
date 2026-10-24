using FastEndpoints;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core.Domain;
using LowPressureZone.Data;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PutPerformer(DataContext dataContext, PerformerPermissions permissions)
    : EndpointWithMapper<PerformerRequest, PerformerMapper>
{
    public override void Configure()
    {
        Put("/performers/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await dataContext.Performers
                                         .Where(p => p.Id == id)
                                         .FirstOrDefaultAsync(ct);

        if (performer is null || PerformerPermissions.IsHiddenFromApi(performer))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (!permissions.IsEditAuthorized(performer))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = DomainResult.Compose(performer.ChangeName(req.Name),
                                          performer.ChangeSocialUrl(req.SocialUrl));
        
        await this.PublishOrThrowAsync(result);

        await dataContext.SaveChangesAsync(ct);
        
        await Send.NoContentAsync(ct);
    }
}