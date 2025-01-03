namespace LowPressureZone.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
}
