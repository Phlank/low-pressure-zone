using System.Globalization;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

namespace LowPressureZone.Api.IntegrationTests.Schedules.HourlySlots;

internal static class HourlySlotTestData
{
    public static HourlySlotRequest Request(Guid performerId, DateTimeOffset startsAt, int duration = 1, string? subtitle = null)
        => new()
        {
            PerformerId = performerId,
            StartsAt = startsAt,
            Duration = duration,
            Subtitle = subtitle
        };

    public static string Route(Guid scheduleId, Guid? slotId = null)
        => slotId is null
               ? $"/api/schedules/{scheduleId}/hourlySlots"
               : $"/api/schedules/{scheduleId}/hourlySlots/{slotId}";

    public static MultipartFormDataContent AsForm(HourlySlotRequest request)
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(request.PerformerId.ToString()), nameof(request.PerformerId));
        form.Add(new StringContent(request.StartsAt.ToString("O", CultureInfo.InvariantCulture)),
                 nameof(request.StartsAt));
        form.Add(new StringContent(request.Duration.ToString(CultureInfo.InvariantCulture)),
                 nameof(request.Duration));
        if (request.Subtitle is not null)
            form.Add(new StringContent(request.Subtitle), nameof(request.Subtitle));
        return form;
    }
}
