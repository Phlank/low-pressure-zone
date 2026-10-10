namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required Guid CommunityId { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public bool IsHourlyAllowed { get; set; }
    public bool IsClashAllowed { get; set; }
    public required bool IsVisibleToPublic { get; set; }
}