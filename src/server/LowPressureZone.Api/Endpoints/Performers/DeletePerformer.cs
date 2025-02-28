using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public class DeletePerformer : EndpointWithoutRequest<EmptyResponse>
{
    private readonly DataContext _dataContext;
    private readonly PerformerRules _rules;

    public DeletePerformer(DataContext dataContext, PerformerRules rules)
    {
        _dataContext = dataContext;
        _rules = rules;
    }

    public override void Configure()
    {
        Delete("/performers/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(401)
                                      .Produces(404));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (!_dataContext.Has<Performer>(id))
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        if (!_rules.CanUserDeletePerformer(id))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var deleted = await _dataContext.Performers.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
        await SendNoContentAsync(ct);
    }
}
