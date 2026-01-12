using System.Text.Json.Serialization;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Soundclashes;
using LowPressureZone.Api.Endpoints.Timeslots;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleResponse
{
    public required Guid Id { get; set; }
    public required DateTimeOffset StartsAt { get; set; }
    public required DateTimeOffset EndsAt { get; set; }
    public required string Description { get; set; }
    public required CommunityResponse Community { get; set; }
    public required IEnumerable<TimeslotResponse> Timeslots { get; set; }
    public required IEnumerable<SoundclashResponse> Soundclashes { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
    public required bool IsTimeslotCreationAllowed { get; set; }
    public required bool IsSoundclashCreationAllowed { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ScheduleType Type { get; set; }
}