using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performer;

public sealed class PostPerformer : Endpoint<PerformerRequest, EmptyResponse, PerformerRequestMapper>
{
    public required DataContext DataContext { get; set; }
    public override void Configure()
    {
        Post("/performer");
        Description(b => b.Produces(204)
                          .ProducesProblem(400));
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var isNameInUse = DataContext.Performers.Any(p => p.Name == req.Name);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), "Performer name already in use.", req.Name));
        }

        var performer = Map.ToEntity(req);
        DataContext.Performers.Add(performer);
        DataContext.SaveChanges();
        await SendCreatedAtAsync<GetPerformerById>(new { performer.Id }, Response);
    }
}
