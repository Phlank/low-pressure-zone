using FastEndpoints;

namespace LowPressureZone.Identity.Application.Commands.Lock;

public record LockUserCommand(Guid UserId) : ICommand;