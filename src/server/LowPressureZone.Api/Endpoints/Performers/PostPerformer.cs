using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PostPerformer : Endpoint<PerformerRequest, EmptyResponse, PerformerRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Post("/performers");
        Description(b => b.Produces(201));
        AllowAnonymous();
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var isNameInUse = DataContext.Performers.Any(p => p.Name == req.Name);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), "Name in use", req.Name));
        }

        var performer = Map.ToEntity(req);
        DataContext.Performers.Add(performer);
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetPerformerById>(new { performer.Id }, Response);
    }
}
