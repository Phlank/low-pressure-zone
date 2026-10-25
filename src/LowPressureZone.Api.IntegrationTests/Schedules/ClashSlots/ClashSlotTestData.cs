using LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

namespace LowPressureZone.Api.IntegrationTests.Schedules.ClashSlots;

internal static class ClashSlotTestData
{
    public static ClashSlotRequest Request(
        Guid performerOneId,
        Guid performerTwoId,
        DateTimeOffset startsAt,
        int duration = 2,
        params string[] rounds)
        => new()
        {
            PerformerOneId = performerOneId,
            PerformerTwoId = performerTwoId,
            StartsAt = startsAt,
            Duration = duration,
            Rounds = [.. rounds]
        };

    public static string Route(Guid scheduleId, Guid? slotId = null)
        => slotId is null
               ? $"/api/schedules/{scheduleId}/clash-slots"
               : $"/api/schedules/{scheduleId}/clash-slots/{slotId}";
}
