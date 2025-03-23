using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public sealed class Schedule : BaseEntity, IDateTimeRange
{
    public required string Description { get; set; } = string.Empty;
    public required Guid CommunityId { get; set; }
    public Community Community { get; set; } = null!;
    public ICollection<Timeslot> Timeslots { get; set; } = [];
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
}
