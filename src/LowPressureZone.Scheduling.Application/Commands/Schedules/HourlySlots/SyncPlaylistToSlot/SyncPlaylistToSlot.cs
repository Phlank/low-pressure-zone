using FastEndpoints;
using LowPressureZone.Core;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncPlaylistTimeToSlot;

public record SyncPlaylistToSlot(Guid Id) : ICommand<Result<string>>;