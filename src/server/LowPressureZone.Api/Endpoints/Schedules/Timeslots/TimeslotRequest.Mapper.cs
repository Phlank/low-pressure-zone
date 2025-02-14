﻿using FastEndpoints;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotRequestMapper : RequestMapper<TimeslotRequest, Timeslot>
{
    public override Timeslot ToEntity(TimeslotRequest r)
    {
        return new Timeslot
        {
            Id = Guid.NewGuid(),
            Name = r.Name,
            Start = r.Start.ToUniversalTime(),
            End = r.End.ToUniversalTime(),
            Type = r.PerformanceType,
            PerformerId = r.PerformerId,
            ScheduleId = Guid.Empty,
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
        };
    }
}
