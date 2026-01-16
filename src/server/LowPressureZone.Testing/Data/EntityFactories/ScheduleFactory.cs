using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Testing.Data.EntityFactories;

public static class ScheduleFactory
{
    public static Schedule Create(
        Guid? id = null,
        Guid? communityId = null,
        Community? community = null,
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        string? description = null,
        ScheduleType? type = null) =>
        new()
        {
            Id = id ?? Guid.Empty,
            CommunityId = communityId ?? community?.Id ?? Guid.Empty,
            Community = community ?? null!,
            StartsAt = startsAt ?? DateTimeOffset.UtcNow,
            EndsAt = endsAt ?? DateTimeOffset.UtcNow.AddHours(1),
            Description = description ?? "Test Schedule",
            Type = type ?? ScheduleType.Hourly
        };
}