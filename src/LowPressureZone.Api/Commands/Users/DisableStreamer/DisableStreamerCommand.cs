using FastEndpoints;

namespace LowPressureZone.Api.Commands.Users.DisableStreamer;

public record DisableStreamerCommand(Guid UserId) : ICommand;