using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Performers;

public sealed class PutPerformer : Endpoint<PerformerRequest, EmptyResponse, PerformerRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/performers/{id}");
        Description(b => b.Produces(204)
                          .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(PerformerRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var existingPerformer = DataContext.Performers.Find(id);
        if (existingPerformer == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var isNameInUse = DataContext.Performers.Any(p => p.Name == req.Name && p.Id != id);
        if (isNameInUse)
        {
            ThrowError(new ValidationFailure(nameof(req.Name), "Name in use", req.Name));
        }

        if (req.Name != existingPerformer.Name || req.Url != existingPerformer.Url)
        {
            existingPerformer.Name = req.Name;
            existingPerformer.Url = req.Url;
            existingPerformer.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendNoContentAsync(ct);
    }
}
