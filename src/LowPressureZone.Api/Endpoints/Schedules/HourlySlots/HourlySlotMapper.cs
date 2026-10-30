using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

[RegisterService<HourlySlotMapper>(LifeTime.Singleton)]
public sealed class HourlySlotMapper(
    HourlySlotPermissions permissions)
    : IResponseMapper
{
    public HourlySlotResponse FromEntity(HourlySlot slot) => new()
    {
        Id = slot.Id,
        ScheduleId = slot.ScheduleId,
        StartsAt = slot.TimeRange.StartsAt,
        EndsAt = slot.TimeRange.EndsAt,
        Duration = slot.TimeRange.Duration,
        Subtitle = slot.Subtitle,
        PerformerId = slot.PerformerId,
        UploadedFileName = slot.Prerecord.UploadedFileName,
        IsPrerecorded = slot.Prerecord.IsPrerecorded,
        ReminderTime = slot.Reminder.ReminderTime,
        IsEditable = permissions.IsEditAuthorized(slot),
        IsDeletable = permissions.IsDeleteAuthorized(slot)
    };
}