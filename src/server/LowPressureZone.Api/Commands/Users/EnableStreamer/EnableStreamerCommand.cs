using FastEndpoints;

namespace LowPressureZone.Api.Commands.Users.EnableStreamer;

public record EnableStreamerCommand(Guid UserId) : ICommand;