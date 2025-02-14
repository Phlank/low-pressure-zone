using FastEndpoints;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class GetSchedulesRequest
{
    public DateTime? Before { get; set; }
    public DateTime? After { get; set; }
}
