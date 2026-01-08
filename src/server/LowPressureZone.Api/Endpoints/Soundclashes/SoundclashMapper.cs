using FastEndpoints;
using LowPressureZone.Api.Endpoints.Performers;
using LowPressureZone.Api.Rules;
using LowPressureZone.Domain.Entities;
using Shouldly;

namespace LowPressureZone.Api.Endpoints.Soundclashes;

public class SoundclashMapper(SoundclashRules rules, PerformerMapper performerMapper) : IRequestMapper, IResponseMapper
{
    public Soundclash FromRequest(SoundclashRequest req) =>
        new()
        {
            ScheduleId = req.ScheduleId,
            PerformerOneId = req.PerformerOneId,
            PerformerTwoId = req.PerformerTwoId,
            RoundOne = req.RoundOne.Trim(),
            RoundTwo = req.RoundTwo.Trim(),
            RoundThree = req.RoundThree.Trim(),
            StartsAt = req.StartsAt.ToUniversalTime(),
            EndsAt = req.EndsAt.ToUniversalTime()
        };

    public SoundclashResponse ToResponse(Soundclash soundclash)
    {
        soundclash.PerformerOne.ShouldNotBeNull();
        soundclash.PerformerTwo.ShouldNotBeNull();
        return new SoundclashResponse
        {
            Id = soundclash.Id,
            ScheduleId = soundclash.ScheduleId,
            PerformerOne = performerMapper.FromEntity(soundclash.PerformerOne),
            PerformerTwo = performerMapper.FromEntity(soundclash.PerformerTwo),
            RoundOne = soundclash.RoundOne,
            RoundTwo = soundclash.RoundTwo,
            RoundThree = soundclash.RoundThree,
            StartsAt = soundclash.StartsAt,
            EndsAt = soundclash.EndsAt,
            IsEditable = rules.IsEditAuthorized(soundclash),
            IsDeletable = rules.IsDeleteAuthorized(soundclash)
        };
    }
}