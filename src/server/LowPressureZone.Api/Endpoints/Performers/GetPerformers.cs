using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class GetPerformers : EndpointWithoutRequest<IEnumerable<PerformerResponse>, PerformerMapper>
{
    private readonly DataContext _dataContext;
    private readonly PerformerRules _rules;

    public GetPerformers(DataContext dataContext, PerformerRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Get("/performers");
        Description(b => b.Produces<List<PerformerResponse>>(200));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var performers = await _dataContext.Performers.AsNoTracking()
                                                     .OrderBy(p => p.Name)
                                                     .ToListAsync(ct);
        performers.RemoveAll(_rules.IsHiddenFromApi);
        var responses = performers.Select(Map.FromEntity);
        await SendOkAsync(responses, ct);
    }
}
