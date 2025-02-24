using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public class Schedule : BaseEntity, IDateTimeRange
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required string Description { get; set; } = string.Empty;
    public required Guid AudienceId { get; set; }
    public virtual Audience? Audience { get; set; }
    public virtual List<Timeslot> Timeslots { get; set; } = new();
}
