using LowPressureZone.Api.Constants;
using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class TimeslotFactory
{
    public static Timeslot Create(
        Guid? id = null,
        string? type = null,
        Guid? performerId = null,
        Performer? performer = null,
        Guid? scheduleId = null,
        Schedule? schedule = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        string? subtitle = null,
        int? azuracastMediaId = null,
        string? uploadedFileName = null) => new Timeslot
    {
        Id = id ?? Guid.Empty,
        Type = type ?? PerformanceTypes.Live,
        PerformerId = performerId ?? Guid.Empty,
        Performer = performer ?? null!,
        ScheduleId = scheduleId ?? Guid.Empty,
        Schedule = schedule ?? null!,
        StartsAt = startsAt ?? DateTimeOffset.UtcNow.AddHours(1),
        EndsAt = endsAt ?? DateTimeOffset.UtcNow.AddHours(2),
        Subtitle = subtitle ?? "Test Subtitle",
        UploadedFileName = uploadedFileName ?? null,
        AzuraCastMediaId = azuracastMediaId ?? null
    };
}