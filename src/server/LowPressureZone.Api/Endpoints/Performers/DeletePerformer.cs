using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Performers;

public class DeletePerformer : EndpointWithoutRequest<EmptyResponse>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Delete("/performers/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var deleted = DataContext.Performers.Where(p => p.Id == id).ExecuteDelete();
        if (deleted > 0)
        {
            await SendNoContentAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}
