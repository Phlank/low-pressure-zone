using LowPressureZone.Domain.Enums;
using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public sealed class Schedule : BaseEntity, IDateTimeRange
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Guid CommunityId { get; set; }
    public Community Community { get; init; } = null!;
    public ICollection<Timeslot> Timeslots { get; init; } = [];
    public ICollection<Soundclash> Soundclashes { get; init; } = [];
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public required ScheduleType Type { get; init; }
    public required bool IsOrganizersOnly { get; set; }
}