using System.Globalization;
using FastEndpoints;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Core;
using LowPressureZone.Infrastructure.Abstraction;
using LowPressureZone.Infrastructure.Extensions;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate.HourlySlotEntity;
using Results = LowPressureZone.Core.Results;

namespace LowPressureZone.Scheduling.Application.Converters;

[RegisterService<IConverter<HourlySlot, StationPlaylist>>(LifeTime.Scoped)]
public sealed class HourlySlotRequestToAzuraCastPlaylistConverter : IConverter<HourlySlot, StationPlaylist>
{
    public Result<StationPlaylist, ValidationFailure> Convert(HourlySlot slot)
    {
        var title = string.IsNullOrWhiteSpace(slot.Subtitle)
                        ? slot.Schedule.TimeRange.StartsAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : slot.Subtitle;

        var playlistEnd = slot.TimeRange.StartsAt.AddHours(slot.TimeRange.Duration).AddMinutes(5);

        return Results.Ok<StationPlaylist, ValidationFailure>(new StationPlaylist
        {
            IsEnabled = true,
            Name = $"Prerecorded Slot - {slot.Performer.Name} - {title}",
            Type = StationPlaylistType.Default,
            Order = StationPlaylistOrder.Sequential,
            Source = StationPlaylistSource.Songs,
            Weight = 1,
            BackendOptions = [StationPlaylistBackendOption.Interrupt, StationPlaylistBackendOption.SingleTrack],
            ScheduleItems =
            [
                new StationPlaylistScheduleItem
                {
                    Days = [],
                    StartDate = DateOnly.FromDateTime(slot.TimeRange.StartsAt.UtcDateTime),
                    StartTime = slot.TimeRange.StartsAt.ToIso8601TimeInt(),
                    EndDate = DateOnly.FromDateTime(playlistEnd.UtcDateTime),
                    EndTime = playlistEnd.ToIso8601TimeInt(),
                    LoopOnce = true
                }
            ]
        });
    }
}