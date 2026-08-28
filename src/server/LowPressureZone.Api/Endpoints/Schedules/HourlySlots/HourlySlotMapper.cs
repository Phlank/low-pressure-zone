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
        StartsAt = slot.StartsAt,
        EndsAt = slot.EndsAt,
        Subtitle = slot.Subtitle,
        Performer = performerMapper.FromEntity(slot.Performer),
        PerformanceType = "Hourly",
        UploadedFileName = slot.Prerecord.UploadedFileName,
        IsEditable = rules.IsEditAuthorized(slot),
        IsDeletable = rules.IsDeleteAuthorized(slot)
    };
}