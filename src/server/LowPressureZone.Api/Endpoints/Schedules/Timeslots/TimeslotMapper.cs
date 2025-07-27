using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public class TimeslotMapper(
    IHttpContextAccessor contextAccessor,
    TimeslotRules rules,
    PerformerMapper performerMapper)
    : IRequestMapper, IResponseMapper
{
    public Timeslot ToEntity(TimeslotRequest req)
        => new()
        {
            Name = req.Name?.Trim(),
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime(),
            Type = req.PerformanceType.Trim(),
            PerformerId = req.PerformerId,
            ScheduleId = contextAccessor.GetGuidRouteParameterOrDefault("scheduleId")
        };

    public async Task UpdateEntityAsync(TimeslotRequest req, Timeslot timeslot, CancellationToken ct = default)
    {
        var dataContext = contextAccessor.Resolve<DataContext>();
        timeslot.StartsAt = req.StartsAt;
        timeslot.EndsAt = req.EndsAt;
        timeslot.PerformerId = req.PerformerId;
        timeslot.Type = req.PerformanceType;
        timeslot.Name = req.Name;
        if (!dataContext.ChangeTracker.HasChanges()) return;
        timeslot.LastModifiedDate = DateTime.UtcNow;
        await dataContext.SaveChangesAsync(ct);
    }

    public TimeslotResponse FromEntity(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        return new TimeslotResponse
        {
            Id = timeslot.Id,
            StartsAt = timeslot.StartsAt,
            EndsAt = timeslot.EndsAt,
            Name = timeslot.Name,
            Performer = performerMapper.FromEntity(timeslot.Performer),
            PerformanceType = timeslot.Type,
            IsEditable = rules.IsEditAuthorized(timeslot),
            IsDeletable = rules.IsDeleteAuthorized(timeslot)
        };
    }
}