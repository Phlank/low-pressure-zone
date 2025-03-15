using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Communities;

public class DeleteCommunity : EndpointWithoutRequest
{
    private readonly DataContext _dataContext;
    private readonly CommunityRules _rules;

    public DeleteCommunity(DataContext dataContext, CommunityRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Delete("/communities/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var community = await _dataContext.Communities.AsNoTracking()
                                                   .Where(a => a.Id == id)
                                                   .FirstOrDefaultAsync(ct);

        if (community == null || community.IsDeleted)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!_rules.IsDeleteAuthorized(community))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _dataContext.Schedules.Where(s => s.StartsAt > DateTime.UtcNow).ExecuteDeleteAsync(ct);
        community.IsDeleted = true;
        community.LastModifiedDate = DateTime.UtcNow;
        await _dataContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
