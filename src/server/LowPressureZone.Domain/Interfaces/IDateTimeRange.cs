namespace LowPressureZone.Domain.Interfaces;

public interface IDateTimeRange
{
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
}