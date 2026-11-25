using FastEndpoints;
using LowPressureZone.Core;

namespace LowPressureZone.Scheduling.Application.Commands.Schedules.HourlySlots.SyncMediaMetadataToSlot;

public record SyncMediaMetadataToSlot(Guid SlotId) : ICommand, ICommand<Result<string>>;