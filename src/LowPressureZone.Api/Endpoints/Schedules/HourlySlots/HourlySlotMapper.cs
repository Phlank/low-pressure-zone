using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Rules;
using HourlySlot = LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.HourlySlot;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

[RegisterService<HourlySlotMapper>(LifeTime.Singleton)]
public sealed class HourlySlotMapper(
    HourlySlotRules rules,
    PerformerMapper performerMapper)
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
        IsEditable = rules.IsEditAuthorized(slot),
        IsDeletable = rules.IsDeleteAuthorized(slot)
    };
}