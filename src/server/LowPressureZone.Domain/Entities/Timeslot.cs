using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public class Timeslot : BaseEntity, IDateTimeRange
{
    public string? Name { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required string Type { get; set; }
    public required Guid PerformerId { get; set; }
    public virtual Performer? Performer { get; set; }
    public required Guid ScheduleId { get; set; }
    public virtual Schedule? Schedule { get; set; }
}