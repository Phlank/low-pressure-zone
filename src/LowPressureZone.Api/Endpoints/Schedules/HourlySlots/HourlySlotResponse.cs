using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class HourlySlotResponse : ITimeRange
{
    public required Guid Id { get; set; }
    public required Guid ScheduleId { get; set; }
    public required Guid PerformerId { get; set; }
    public string? Subtitle { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public int Duration { get; set; }
    public required string? UploadedFileName { get; set; }
    public required bool IsPrerecorded { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
    public string Type => "Hourly";
}