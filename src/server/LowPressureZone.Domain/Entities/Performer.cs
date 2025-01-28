namespace LowPressureZone.Domain.Entities;

public class Performer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}