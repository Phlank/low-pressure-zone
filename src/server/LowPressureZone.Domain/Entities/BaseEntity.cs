namespace LowPressureZone.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public string LastModifiedBy { get; set; } = string.Empty;
}
