using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public sealed class GetAudiences(DataContext dataContext) : EndpointWithoutRequest<IEnumerable<AudienceResponse>, AudienceMapper>
{
    public override void Configure()
    {
        Get("/audiences");
        Description(b => b.Produces<List<AudienceResponse>>(200));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var audiencesQuery = dataContext.Audiences.Include(a => a.Relationships).AsNoTracking();
        if (!User.IsInRole(RoleNames.Admin))
        {
            audiencesQuery = audiencesQuery.Where(a => a.IsDeleted);
        }
        var audiences = await audiencesQuery.ToListAsync(ct);

        var responses = audiences.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
