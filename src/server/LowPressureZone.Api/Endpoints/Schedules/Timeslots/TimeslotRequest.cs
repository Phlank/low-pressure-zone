﻿using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public sealed class TimeslotRequest : IDateTimeRange
{
    public required Guid PerformerId { get; set; }
    public required string PerformanceType { get; set; }
    public string? Name { get; set; }
    public IFormFile? File { get; set; }
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
}
