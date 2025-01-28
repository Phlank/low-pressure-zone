namespace LowPressureZone.Domain.Entities;

public class Schedule : BaseEntity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public List<Timeslot> Timeslots { get; set; } = new List<Timeslot>();
    public Audience Audience { get; set; } = new();
}
