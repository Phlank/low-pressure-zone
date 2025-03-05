using FastEndpoints;
using LowPressureZone.Api.Endpoints.Audiences;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules;

public class ScheduleMapper(AudienceMapper audienceMapper,
                            TimeslotMapper timeslotMapper,
                            ScheduleRules rules)
    : Mapper<ScheduleRequest, ScheduleResponse, Schedule>, IRequestMapper, IResponseMapper
{
    public override Schedule ToEntity(ScheduleRequest req)
    {
        return new Schedule
        {
            Id = Guid.NewGuid(),
            AudienceId = req.AudienceId,
            Description = req.Description,
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime()
        };
    }

    public override Task<Schedule> ToEntityAsync(ScheduleRequest req, CancellationToken ct = default)
        => Task.FromResult(ToEntity(req));

    public override async Task<Schedule> UpdateEntityAsync(ScheduleRequest req, Schedule schedule, CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        schedule.StartsAt = req.StartsAt;
        schedule.EndsAt = req.EndsAt;
        schedule.AudienceId = req.AudienceId;
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
        schedule.Audience.ShouldNotBeNull();
        schedule.Timeslots.ShouldNotBeNull();
        schedule.Timeslots.ForEach(t => t.Performer.ShouldNotBeNull());

        return new ScheduleResponse
        {
            Id = schedule.Id,
            StartsAt = schedule.StartsAt,
            EndsAt = schedule.EndsAt,
            Description = schedule.Description,
            Audience = audienceMapper.FromEntity(schedule.Audience!),
            Timeslots = schedule.Timeslots.Select(timeslotMapper.FromEntity),
            IsEditable = rules.IsEditAuthorized(schedule),
            IsDeletable = rules.IsDeleteAuthorized(schedule),
            IsTimeslotCreationAllowed = rules.IsAddingTimeslotsAuthorized(schedule)
        };
    }

    public override Task<ScheduleResponse> FromEntityAsync(Schedule schedule, CancellationToken ct = default)
        => Task.FromResult(FromEntity(schedule));
}
