using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Schedules.Timeslots;

public sealed class TimeslotMapper(
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
            ScheduleId = contextAccessor.GetGuidRouteParameterOrDefault("scheduleId"),
            UploadedFileName = req.File?.FileName
        };
    
    public void UpdateEntity(TimeslotRequest req, Timeslot timeslot)
    {
        timeslot.Name = req.Name?.Trim();
        timeslot.StartsAt = req.StartsAt.ToUniversalTime();
        timeslot.EndsAt = req.EndsAt.ToUniversalTime();
        timeslot.Type = req.PerformanceType.Trim();
        timeslot.PerformerId = req.PerformerId;
        if (req.File != null)
            timeslot.UploadedFileName = req.File.FileName;
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
            UploadedFileName = timeslot.UploadedFileName,
            IsEditable = rules.IsEditAuthorized(timeslot),
            IsDeletable = rules.IsDeleteAuthorized(timeslot)
        };
    }
}