using FastEndpoints;
using LowPressureZone.Scheduling.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.Commands.Broadcasts.SyncBroadcasts;

public record SyncBroadcastsCommand() : ICommand<List<Broadcast>>;