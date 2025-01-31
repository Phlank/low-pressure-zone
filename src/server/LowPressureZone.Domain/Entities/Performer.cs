namespace LowPressureZone.Domain.Entities;

public class Performer : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
}