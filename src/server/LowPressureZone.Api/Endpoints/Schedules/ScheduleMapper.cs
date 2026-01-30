using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Soundclashes;
using LowPressureZone.Api.Endpoints.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

public sealed class ScheduleMapper(
    IHttpContextAccessor accessor,
    CommunityMapper communityMapper,
    TimeslotMapper timeslotMapper,
    SoundclashMapper soundclashMapper,
    ScheduleRules rules)
    : IRequestMapper, IResponseMapper
{
    public Schedule ToEntity(ScheduleRequest req)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            CommunityId = req.CommunityId,
            Description = req.Description,
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime(),
            Type = req.Type,
            IsOrganizersOnly = req.IsOrganizersOnly
        };

    public ScheduleResponse FromEntity(Schedule schedule)
    {
        schedule.Community.ShouldNotBeNull();
        schedule.Timeslots.ShouldNotBeNull();
        foreach (var timeslot in schedule.Timeslots) timeslot.Performer.ShouldNotBeNull();

        return new ScheduleResponse
        {
            Id = schedule.Id,
            StartsAt = schedule.StartsAt,
            EndsAt = schedule.EndsAt,
            Name = schedule.Name,
            Description = schedule.Description,
            Community = communityMapper.FromEntity(schedule.Community),
            Timeslots = schedule.Timeslots.Select(timeslotMapper.FromEntity),
            Soundclashes = schedule.Soundclashes.Select(soundclashMapper.FromEntity),
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsTimeslotCreationAllowed = rules.IsAddingTimeslotsAllowed(schedule),
            IsSoundclashCreationAllowed = rules.IsAddingSoundclashesAllowed(schedule),
            IsOrganizersOnly = schedule.IsOrganizersOnly,
            Type = schedule.Type
        };
    }
}