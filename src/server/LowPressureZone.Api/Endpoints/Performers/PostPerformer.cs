using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Api.Constants;
using LowPressureZone.Domain;
using LowPressureZone.Domain.BusinessRules;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Extensions;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PostPerformer : EndpointWithMapper<PerformerRequest, PerformerRequestMapper>
{
    private readonly DataContext _dataContext;

    public PostPerformer(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public override void Configure()
    {
        Post("/performers");
        Description(b => b.Produces(201));
        Roles(RoleNames.All);
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var performer = Map.ToEntity(req);
        _dataContext.Performers.Add(performer);
        _dataContext.SaveChanges();
        await SendCreatedAtAsync<GetPerformerById>(new { performer.Id }, Response);
    }
}
