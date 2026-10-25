using LowPressureZone.Api.Endpoints.Schedules;

namespace LowPressureZone.Api.IntegrationTests.Schedules;

internal static class ScheduleTestData
{
    public static DateTimeOffset Start => DateTimeOffset.UtcNow.AddDays(10);

    public static ScheduleRequest Request(
        Guid communityId,
        string name = "Schedule",
        DateTimeOffset? startsAt = null,
        DateTimeOffset? endsAt = null,
        bool isHourlyAllowed = true,
        bool isClashAllowed = true,
        bool isVisibleToPublic = false)
    {
        var start = startsAt ?? Start;
        return new ScheduleRequest
        {
            Name = name,
            Description = "Description",
            CommunityId = communityId,
            StartsAt = start,
            EndsAt = endsAt ?? start.AddHours(8),
            IsHourlyAllowed = isHourlyAllowed,
            IsClashAllowed = isClashAllowed,
            IsVisibleToPublic = isVisibleToPublic
        };
    }

    public static string Route(Guid id) => $"/api/schedules/{id}";
}
