namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

public class HourlySlotRequest
{
    public required Guid PerformerId { get; set; }
    public string? Subtitle { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required int Duration { get; set; }
    public bool ReplaceMedia { get; set; }
    public bool DeleteMedia { get; set; }
    public IFormFile? File { get; set; }
}