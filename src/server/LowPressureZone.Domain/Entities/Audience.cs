namespace LowPressureZone.Domain.Entities;

public class Audience : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
}