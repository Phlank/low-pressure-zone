using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using LowPressureZone.Domain.Extensions;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotMapper(IHttpContextAccessor contextAccessor,
                            TimeslotRules rules,
                            PerformerMapper performerMapper) 
    : Mapper<TimeslotRequest, TimeslotResponse, Timeslot>, IRequestMapper, IResponseMapper
{
    public override Timeslot ToEntity(TimeslotRequest req)
    {
        return new Timeslot
        {
            Id = Guid.NewGuid(),
            Name = req.Name?.Trim(),
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime(),
            Type = req.PerformanceType.Trim(),
            PerformerId = req.PerformerId,
            ScheduleId = contextAccessor.GetGuidRouteParameterOrDefault("scheduleId"),
            CreatedDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow,
        };
    }

    public override Task<Timeslot> ToEntityAsync(TimeslotRequest req, CancellationToken ct = default)
        => Task.FromResult(ToEntity(req));

    public override async Task<Timeslot> UpdateEntityAsync(TimeslotRequest req, Timeslot timeslot, CancellationToken ct = default)
    {
        var dataContext = Resolve<DataContext>();
        timeslot.StartsAt = req.StartsAt;
        timeslot.EndsAt = req.EndsAt;
        timeslot.PerformerId = req.PerformerId;
        timeslot.Type = req.PerformanceType;
        timeslot.Name = req.Name;
        if (dataContext.ChangeTracker.HasChanges())
        {
            timeslot.LastModifiedDate = DateTime.UtcNow;
            await dataContext.SaveChangesAsync(ct);
        }
        return timeslot;
    }

    public override TimeslotResponse FromEntity(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();

        return new TimeslotResponse
        {
            Id = timeslot.Id,
            Start = timeslot.StartsAt,
            End = timeslot.EndsAt,
            Name = timeslot.Name,
            Performer = performerMapper.FromEntity(timeslot.Performer!),
            PerformanceType = timeslot.Type,
            IsEditable = rules.IsEditAuthorized(timeslot),
            IsDeletable = rules.IsDeleteAuthorized(timeslot)
        };
    }

    public override Task<TimeslotResponse> FromEntityAsync(Timeslot timeslot, CancellationToken ct = default)
        => Task.FromResult(FromEntity(timeslot));
}
