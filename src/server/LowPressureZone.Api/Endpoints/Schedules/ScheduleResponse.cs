using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleResponse
{
    public required Guid Id { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required string Description { get; set; }
    public required CommunityResponse Community { get; set; }
    public required IEnumerable<TimeslotResponse> Timeslots { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
    public required bool IsTimeslotCreationAllowed { get; set; }
}