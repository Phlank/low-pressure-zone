using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class DeleteSchedule : EndpointWithoutRequest<EmptyResponse>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Delete("/schedules/{id}");
        Description(builder => builder.Produces(204)
                                      .Produces(404));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (DataContext.Schedules.Where(s => s.Id == id && s.Timeslots.Any()).Any())
        {
            ThrowError(new ValidationFailure(null, "Cannot delete schedule with timeslots"));
        }
        var deleted = await DataContext.Schedules.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
        if (deleted > 0)
        {
            await SendNoContentAsync();
            return;
        }
        await SendNotFoundAsync();
    }
}
