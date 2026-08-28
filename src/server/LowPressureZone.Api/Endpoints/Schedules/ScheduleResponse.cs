using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleResponse : ITimeRange
{
    public required Guid Id { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required CommunityResponse Community { get; set; }
    public required IEnumerable<ITimeRange> Slots { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
    public required bool IsHourlyAllowed { get; set; }
    public required bool IsClashAllowed { get; set; }
    public required bool IsVisibleToPublic { get; set; }
}