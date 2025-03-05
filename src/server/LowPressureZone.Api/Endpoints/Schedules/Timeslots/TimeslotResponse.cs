﻿using LowPressureZone.Api.Endpoints.Performers;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotResponse
{
    public required Guid Id { get; set; }
    public required PerformerResponse Performer { get; set; }
    public required string PerformanceType { get; set; }
    public string? Name { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
    public required bool IsEditable { get; set; }
    public required bool IsDeletable { get; set; }
}
