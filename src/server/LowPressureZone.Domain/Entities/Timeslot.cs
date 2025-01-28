using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Domain.Entities;

public class Timeslot : BaseEntity
{
    public Performer? Performer { get; set; }
    public PerformanceType Type { get; set; }
    public string? Name { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}