namespace LowPressureZone.Domain.Interfaces;

public interface IDateTimeRange
{
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}