using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public class Schedule : BaseEntity, IDateTimeRange
{
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required string Description { get; set; } = string.Empty;
    public required Guid AudienceId { get; set; }
    public virtual Audience? Audience { get; set; }
    public virtual List<Timeslot> Timeslots { get; set; } = [];
}
