using FastEndpoints;

namespace LowPressureZone.Identity.Application.Commands.EnableStreamer;

public record EnableStreamerCommand(Guid UserId) : ICommand;