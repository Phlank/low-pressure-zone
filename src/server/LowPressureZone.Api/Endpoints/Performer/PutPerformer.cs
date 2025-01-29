﻿using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;

namespace LowPressureZone.Api.Endpoints.Performer;

public sealed class PutPerformer : Endpoint<PerformerRequest, EmptyResponse, PerformerRequestMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Put("/performer/{id}");
        Description(b => b.Produces(200)
                          .ProducesProblem(400)
                          .Produces(404));
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
            ThrowError(new ValidationFailure(nameof(req.Name), "Performer name already in use.", req.Name));
        }

        if (req.Name != existingPerformer.Name || req.Url != existingPerformer.Url)
        {
            existingPerformer.Name = req.Name;
            existingPerformer.Url = req.Url;
            existingPerformer.LastModifiedDate = DateTime.UtcNow;
            DataContext.SaveChanges();
        }

        await SendOkAsync(ct);
    }
}
