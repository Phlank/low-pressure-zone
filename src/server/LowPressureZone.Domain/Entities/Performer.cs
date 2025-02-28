﻿using System.Security.Claims;

namespace LowPressureZone.Domain.Entities;

public class Performer : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public virtual List<Timeslot> Timeslots { get; set; } = new();
    public List<Guid> LinkedUserIds { get; set; } = new();
}