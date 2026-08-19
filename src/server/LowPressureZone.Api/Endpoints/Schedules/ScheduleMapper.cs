using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Soundclashes;
using LowPressureZone.Api.Endpoints.Timeslots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.ScheduleAggregate;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleMapper(
    TimeslotMapper timeslotMapper,
    SoundclashMapper soundclashMapper,
    CommunityMapper communityMapper,
    ScheduleRules rules)
    : IRequestMapper, IResponseMapper
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

        return new ScheduleResponse
        {
            Id = schedule.Id,
            StartsAt = schedule.TimeRange.StartsAt,
            EndsAt = schedule.TimeRange.EndsAt,
            Name = schedule.Name,
            Description = schedule.Description,
            Community = communityMapper.FromEntity(schedule.Community),
            Timeslots = schedule.HourlySlots.Select(timeslotMapper.FromEntity),
            Soundclashes = schedule.ClashSlots.Select(soundclashMapper.FromEntity),
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsTimeslotCreationAllowed = rules.IsAddingTimeslotsAllowed(schedule),
            IsSoundclashCreationAllowed = rules.IsAddingSoundclashesAllowed(schedule),
            IsOrganizersOnly = schedule.IsOrganizersOnly,
            Type = schedule.Type
        };
    }
}