using System.Text.Json.Serialization;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleRequest : IDateTimeRange
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required Guid CommunityId { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ScheduleType Type { get; set; }
}