namespace LowPressureZone.Domain.Entities;

public sealed class News : BaseEntity
{
    public required string Title { get; set; } = string.Empty;

    public required string Body { get; set; } = string.Empty;
}