using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PutPerformer(DataContext dataContext, PerformerRules rules)
    : EndpointWithMapper<PerformerRequest, PerformerMapper>
{
    private readonly DataContext _dataContext = dataContext;
    private readonly PerformerRules _rules = rules;

    public override void Configure()
    {
        Put("/performers/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var performer = await _dataContext.Performers
                                          .Where(p => p.Id == id)
                                          .FirstOrDefaultAsync(ct);

        if (performer == null || PerformerRules.IsHiddenFromApi(performer))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!_rules.IsEditAuthorized(performer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await Map.UpdateEntityAsync(req, performer, ct);
        await SendNoContentAsync(ct);
    }
}
