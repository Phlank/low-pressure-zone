using FastEndpoints;
using LowPressureZone.Domain.BroadcastAggregate;

namespace LowPressureZone.Api.Commands.Broadcasts.SyncBroadcasts;

public record SyncBroadcastsCommand() : ICommand<List<Broadcast>>;