using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public class DeleteAudience : EndpointWithoutRequest
{
    private readonly DataContext _dataContext;
    private readonly AudienceRules _rules;

    public DeleteAudience(DataContext dataContext, AudienceRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Delete("/audiences/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var audience = await _dataContext.Audiences.AsNoTracking()
                                                   .Where(a => a.Id == id)
                                                   .FirstOrDefaultAsync(ct);

        if (audience == null || audience.IsDeleted)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!_rules.IsDeleteAuthorized(audience))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await _dataContext.Schedules.Where(s => s.StartsAt > DateTime.UtcNow).ExecuteDeleteAsync(ct);
        audience.IsDeleted = true;
        audience.LastModifiedDate = DateTime.UtcNow;
        await _dataContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}
