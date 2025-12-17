using System.Globalization;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.ApiSchema;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core;
using LowPressureZone.Core.Interfaces;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Converters;

public sealed class TimeslotRequestToAzuraCastPlaylistConverter(DataContext dataContext)
    : IAsyncConverter<TimeslotRequest, StationPlaylist>
{
    public async Task<Result<StationPlaylist, ValidationFailure>> ConvertAsync(
        TimeslotRequest source,
        CancellationToken ct = default)
    {
        var performer = await dataContext.Performers
                                         .Where(performer => performer.Id == source.PerformerId)
                                         .FirstAsync(ct);
        var schedule = await dataContext.Schedules
                                        .Where(schedule => schedule.StartsAt <= source.StartsAt
                                                           && schedule.EndsAt >= source.EndsAt)
                                        .FirstAsync(ct);

        var title = string.IsNullOrWhiteSpace(source.Name)
                        ? schedule.StartsAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        : source.Name;

        var playlistEnd = source.EndsAt.AddMinutes(5);

        return Result.Ok<StationPlaylist, ValidationFailure>(new()
        {
            IsEnabled = true,
            Name = $"Prerecorded Slot - {performer.Name} - {title}",
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
                    StartDate = DateOnly.FromDateTime(source.StartsAt.UtcDateTime),
                    StartTime = source.StartsAt.ToIso8601TimeInt(),
                    EndDate = DateOnly.FromDateTime(playlistEnd.UtcDateTime),
                    EndTime = playlistEnd.ToIso8601TimeInt(),
                    LoopOnce = true
                }
            ]
        });
    }
}