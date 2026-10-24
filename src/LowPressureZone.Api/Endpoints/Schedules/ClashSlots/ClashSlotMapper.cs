using FastEndpoints;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.ScheduleAggregate.ClashSlotEntity;

namespace LowPressureZone.Api.Endpoints.Schedules.ClashSlots;

[RegisterService<ClashSlotMapper>(LifeTime.Singleton)]
public class ClashSlotMapper(ClashSlotPermissions permissions) : IResponseMapper
{
    public ClashSlotResponse FromEntity(ClashSlot entity) => new()
    {
        Id = entity.Id,
        ScheduleId = entity.ScheduleId,
        PerformerOneId = entity.PerformerOneId,
        PerformerTwoId = entity.PerformerTwoId,
        Rounds = entity.Rounds,
        StartsAt = entity.TimeRange.StartsAt,
        Duration = entity.TimeRange.Duration,
        EndsAt = entity.TimeRange.EndsAt,
        IsEditable = permissions.IsEditAuthorized(entity),
        IsDeletable = permissions.IsDeleteAuthorized(entity)
    };
}