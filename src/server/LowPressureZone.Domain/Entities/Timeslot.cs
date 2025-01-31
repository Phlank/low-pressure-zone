using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Entities;

public class Timeslot : BaseEntity
{
    public required Performer Performer { get; set; }
    public required PerformanceType Type { get; set; }
    public string? Name { get; set; }
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
}