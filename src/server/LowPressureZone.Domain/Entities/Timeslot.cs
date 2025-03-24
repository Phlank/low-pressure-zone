﻿using System.ComponentModel.DataAnnotations;
using LowPressureZone.Domain.Interfaces;

namespace LowPressureZone.Domain.Entities;

public sealed class Timeslot : BaseEntity, IDateTimeRange
{
    [MaxLength(64)]
    public string? Name { get; set; }

    [MaxLength(64)]
    public required string Type { get; set; }

    public required Guid PerformerId { get; set; }
    public Performer Performer { get; set; } = null!;
    public required Guid ScheduleId { get; set; }

    public Schedule Schedule { get; set; } = null!;
    public required DateTime StartsAt { get; set; }
    public required DateTime EndsAt { get; set; }
}
