using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Audiences;

public class DeleteAudience : EndpointWithoutRequest<EmptyResponse>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Delete("/audiences/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (DataContext.Schedules.Any(s => s.AudienceId == id))
        {
            ThrowError(new ValidationFailure(null, "Cannot delete audience linked to schedules"));
        }
        var deleted = await DataContext.Audiences.Where(a => a.Id == id).ExecuteDeleteAsync(ct);
        if (deleted > 0)
        {
            await SendNoContentAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}
