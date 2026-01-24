using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Timeslots;

public sealed class TimeslotMapper(
    TimeslotRules rules,
    PerformerMapper performerMapper)
    : IRequestMapper, IResponseMapper
{
    public Timeslot ToEntity(TimeslotRequest req)
        => new()
        {
            Subtitle = req.Subtitle?.Trim(),
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime(),
            Type = req.PerformanceType.Trim(),
            PerformerId = req.PerformerId,
            ScheduleId = req.ScheduleId,
            UploadedFileName = req.File?.FileName
        };

    public void UpdateEntity(TimeslotRequest req, Timeslot timeslot)
    {
        timeslot.Subtitle = req.Subtitle?.Trim();
        timeslot.StartsAt = req.StartsAt.ToUniversalTime();
        timeslot.EndsAt = req.EndsAt.ToUniversalTime();
        timeslot.Type = req.PerformanceType.Trim();
        timeslot.PerformerId = req.PerformerId;
        timeslot.ScheduleId = req.ScheduleId;
        if (req.File != null)
            timeslot.UploadedFileName = req.File.FileName;
    }

    public TimeslotResponse FromEntity(Timeslot timeslot)
    {
        timeslot.Performer.ShouldNotBeNull();
        return new TimeslotResponse
        {
            Id = timeslot.Id,
            ScheduleId = timeslot.ScheduleId,
            StartsAt = timeslot.StartsAt,
            EndsAt = timeslot.EndsAt,
            Subtitle = timeslot.Subtitle,
            Performer = performerMapper.FromEntity(timeslot.Performer),
            PerformanceType = timeslot.Type,
            UploadedFileName = timeslot.UploadedFileName,
            IsEditable = rules.IsEditAuthorized(timeslot),
            IsDeletable = rules.IsDeleteAuthorized(timeslot)
        };
    }
}