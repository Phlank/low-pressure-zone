using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

[RegisterService<ClashSlotMapper>(LifeTime.Singleton)]
public class ClashSlotMapper(PerformerMapper performerMapper, ClashSlotRules rules) : IResponseMapper
{
    public ClashSlotResponse FromEntity(ClashSlot entity) => new()
    {
        Id = entity.Id,
        ScheduleId = entity.ScheduleId,
        PerformerOne = performerMapper.FromEntity(entity.PerformerOne),
        PerformerTwo = performerMapper.FromEntity(entity.PerformerTwo),
        Rounds = entity.Rounds,
        StartsAt = entity.TimeRange.StartsAt,
        Duration = entity.TimeRange.Duration,
        EndsAt = entity.TimeRange.EndsAt,
        IsEditable = rules.IsEditAuthorized(entity),
        IsDeletable = rules.IsDeleteAuthorized(entity)
    };
}