using FastEndpoints;

namespace LowPressureZone.Identity.Application.Commands.DisableStreamer;

public record DisableStreamerCommand(Guid UserId) : ICommand;