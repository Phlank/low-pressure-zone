namespace LowPressureZone.Domain.Entities;

public class Audience : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required List<Guid> LinkedUserIds { get; set; }
    public virtual List<Schedule> Schedules { get; set; } = new();
}