using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public sealed class Schedule : BaseEntity, IDateTimeRange
{
    public required string Subtitle { get; set; }
    public required Guid CommunityId { get; set; }
    public Community Community { get; init; } = null!;
    public ICollection<Timeslot> Timeslots { get; init; } = [];
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
}