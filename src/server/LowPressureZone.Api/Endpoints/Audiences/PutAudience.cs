using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class PutAudience(DataContext dataContext) : EndpointWithMapper<AudienceRequest, AudienceMapper>
{
    public override void Configure()
    {
        Put("/audiences/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(AudienceRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var audience = await dataContext.Audiences.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (audience == null || (audience.IsDeleted && !User.IsInRole(RoleNames.Admin)))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(req, audience, ct);
        await SendNoContentAsync(ct);
    }
}
