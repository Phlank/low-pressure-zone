using FastEndpoints;
using LowPressureZone.Api.Endpoints.Communities;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleMapper(
    IHttpContextAccessor accessor,
    CommunityMapper communityMapper,
    TimeslotMapper timeslotMapper,
    ScheduleRules rules)
    : IRequestMapper, IResponseMapper
{
    public Schedule ToEntity(ScheduleRequest req)
        => new()
        {
            Id = Guid.NewGuid(),
            CommunityId = req.CommunityId,
            Description = req.Description,
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime()
        };

    public async Task UpdateEntityAsync(
        ScheduleRequest req,
        Schedule schedule,
        CancellationToken ct = default)
    {
        var dataContext = accessor.Resolve<DataContext>();
        schedule.StartsAt = req.StartsAt;
        schedule.EndsAt = req.EndsAt;
        schedule.CommunityId = req.CommunityId;
        schedule.Description = req.Description;
        if (!dataContext.ChangeTracker.HasChanges()) return;
        schedule.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
    }

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
            Description = schedule.Description,
            Community = communityMapper.FromEntity(schedule.Community),
            Timeslots = schedule.Timeslots.Select(timeslotMapper.FromEntity),
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsTimeslotCreationAllowed = rules.IsAddingTimeslotsAuthorized(schedule)
        };
    }
}