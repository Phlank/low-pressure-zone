using LowPressureZone.Domain.Enums;
using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public class Timeslot : BaseEntity, IDateTimeRange
{
    public string? Name { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required PerformanceType Type { get; set; }
    public required Guid PerformerId { get; set; }
    public virtual Performer? Performer { get; set; }
    public required Guid ScheduleId { get; set; }
    public virtual Schedule? Schedule { get; set; }
}