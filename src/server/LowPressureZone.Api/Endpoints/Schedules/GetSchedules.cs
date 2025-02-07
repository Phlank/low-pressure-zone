using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.QueryableExtensions;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class GetSchedules : EndpointWithoutRequest<IEnumerable<ScheduleResponse>, ScheduleResponseMapper>
{
    public required DataContext DataContext { get; set; }

    public override void Configure()
    {
        Get("/schedules");
        Description(b => b.Produces<IEnumerable<ScheduleResponse>>(200));
        Roles(RoleNames.AllRoles);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var schedules = DataContext.Schedules.IncludeConnectingProperties().AsNoTracking().ToList();
        await SendOkAsync(schedules.Select(Map.FromEntity), ct);
    }
}
