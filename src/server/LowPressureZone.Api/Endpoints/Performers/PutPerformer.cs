using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PutPerformer : EndpointWithMapper<PerformerRequest, PerformerRequestMapper>
{
    private readonly DataContext _dataContext;
    private readonly PerformerRules _rules;

    public PutPerformer(DataContext dataContext, PerformerRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Put("/performers/{id}");
        Description(b => b.Produces(204)
                          .Produces(401)
                          .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var existingPerformer = await _dataContext.Performers.FindAsync(id);
        if (existingPerformer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (!_rules.CanUserEditPerformer(existingPerformer))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        existingPerformer.Name = req.Name;
        existingPerformer.Url = req.Url;
        if (_dataContext.ChangeTracker.HasChanges())
        {
            existingPerformer.LastModifiedDate = DateTime.UtcNow;
            _dataContext.SaveChanges();
        }

        await SendNoContentAsync(ct);
    }
}
