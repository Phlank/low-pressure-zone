namespace LowPressureZone.Domain.Entities;

public class Schedule : BaseEntity
{
    public required DateTime Start { get; set; }
    public required DateTime End { get; set; }
    public required Guid AudienceId { get; set; }
    public virtual Audience? Audience { get; set; }
    public virtual List<Timeslot> Timeslots { get; set; } = new();
}
