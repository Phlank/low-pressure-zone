using System.Globalization;
using System.Text;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;

namespace LowPressureZone.Scheduling.Application.Extensions;

public static class HourlySlotExtensions
{
    extension(HourlySlot slot)
    {
        public string? TrackTitle
        {
            get {
                if (slot.PrerecordedMix?.UploadedMedia is null)
                    return null;
                
                var builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(slot.Subtitle))
                    builder.Append($"{slot.Subtitle} - ");
                builder.Append(slot.TimeRange.StartsAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }
    }
}