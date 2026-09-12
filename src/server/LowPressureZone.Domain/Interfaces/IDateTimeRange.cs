namespace LowPressureZone.Domain.Interfaces;

public interface IDateTimeRange
{
    DateTimeOffset StartsAt { get; set; }
    DateTimeOffset EndsAt { get; set; }
}