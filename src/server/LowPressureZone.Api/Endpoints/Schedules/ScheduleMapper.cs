using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleMapper(
    CommunityMapper communityMapper,
    TimeslotMapper timeslotMapper,
    ScheduleRules rules)
    : Mapper<ScheduleRequest, ScheduleResponse, Schedule>, IRequestMapper, IResponseMapper
{
    public override Schedule ToEntity(ScheduleRequest req)
    {
        return new Schedule
        {
            Id = Guid.NewGuid(),
            CommunityId = req.CommunityId,
            Description = req.Description,
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime()
        };
    }

    public override Task<Schedule> ToEntityAsync(ScheduleRequest req, CancellationToken ct = default)
    {
        return Task.FromResult(ToEntity(req));
    }

    public override async Task<Schedule> UpdateEntityAsync(ScheduleRequest req, Schedule schedule,
        CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        schedule.StartsAt = req.StartsAt;
        schedule.EndsAt = req.EndsAt;
        schedule.CommunityId = req.CommunityId;
        schedule.Description = req.Description;
        if (dataContext.ChangeTracker.HasChanges())
        {
            schedule.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }

        return schedule;
    }

    public override ScheduleResponse FromEntity(Schedule schedule)
    {
        schedule.Community.ShouldNotBeNull();
        schedule.Timeslots.ShouldNotBeNull();
        foreach (var timeslot in schedule.Timeslots)
        {
            timeslot.Performer.ShouldNotBeNull();
        }

        return new ScheduleResponse
        {
            Id = schedule.Id,
            StartsAt = schedule.StartsAt,
            EndsAt = schedule.EndsAt,
            Description = schedule.Description,
            Community = communityMapper.FromEntity(schedule.Community!),
            Timeslots = schedule.Timeslots.Select(timeslotMapper.FromEntity),
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsTimeslotCreationAllowed = rules.IsAddingTimeslotsAuthorized(schedule)
        };
    }

    public override Task<ScheduleResponse> FromEntityAsync(Schedule schedule, CancellationToken ct = default)
    {
        return Task.FromResult(FromEntity(schedule));
    }
}
