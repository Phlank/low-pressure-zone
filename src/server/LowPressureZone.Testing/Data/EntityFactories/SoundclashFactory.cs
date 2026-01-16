using LowPressureZone.Domain.Entities;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class SoundclashFactory
{
    public static Soundclash Create(
        Guid? id = null,
        Guid? scheduleId = null,
        Schedule? schedule = null,
        Guid? performerOneId = null,
        Performer? performerOne = null,
        Guid? performerTwoId = null,
        Performer? performerTwo = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        string? roundOne = null,
        string? roundTwo = null,
        string? roundThree = null) =>
        new()
        {
            Id = id ?? Guid.Empty,
            ScheduleId = scheduleId ?? schedule?.Id ?? Guid.Empty,
            Schedule = schedule ?? null!,
            PerformerOneId = performerOneId ?? performerOne?.Id ?? Guid.Empty,
            PerformerOne = performerOne ?? null!,
            PerformerTwoId = performerTwoId ?? performerTwo?.Id ?? Guid.Empty,
            PerformerTwo = performerTwo ?? null!,
            StartsAt = startsAt ?? DateTimeOffset.UtcNow,
            EndsAt = endsAt ?? DateTimeOffset.UtcNow.AddHours(2),
            RoundOne = roundOne ?? "Test Round One",
            RoundTwo = roundTwo ?? "Test Round Two",
            RoundThree = roundThree ?? "Test Round Three",
        };
}