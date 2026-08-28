using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Schedules.ClashSlots;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Core;
using LowPressureZone.Domain.ScheduleAggregate;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

[RegisterService<ScheduleMapper>(LifeTime.Singleton)]
public sealed class ScheduleMapper(
    HourlySlotMapper hourlySlotMapper,
    ClashSlotMapper clashSlotMapper,
    CommunityMapper communityMapper,
    ScheduleRules rules)
    : IResponseMapper
{
    public ScheduleResponse FromEntity(Schedule schedule)
    {
        foreach (var slot in schedule.HourlySlots)
        {
            slot.Performer.ShouldNotBeNull();
        }

        foreach (var slot in schedule.ClashSlots)
        {
            slot.PerformerOne.ShouldNotBeNull();
            slot.PerformerTwo.ShouldNotBeNull();
        }

        List<ITimeRange> slots =
        [
            .. schedule.HourlySlots.Select(hourlySlotMapper.FromEntity),
            .. schedule.ClashSlots.Select(clashSlotMapper.FromEntity)
        ];

        return new ScheduleResponse
        {
            Id = schedule.Id,
            StartsAt = schedule.TimeRange.StartsAt,
            EndsAt = schedule.TimeRange.EndsAt,
            Name = schedule.Name,
            Description = schedule.Description,
            Community = communityMapper.FromEntity(schedule.Community),
            Slots = slots.OrderBy(x => x.StartsAt),
            IsVisibleToPublic = schedule.IsVisibleToPublic,
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsHourlyAllowed = rules.IsAddingHourlySlotsAllowed(schedule),
            IsClashAllowed = rules.IsAddingClashSlotsAllowed(schedule),
        };
    }
}