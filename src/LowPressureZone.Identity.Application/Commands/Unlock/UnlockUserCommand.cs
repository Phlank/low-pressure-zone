using FastEndpoints;

namespace LowPressureZone.Identity.Application.Commands.Unlock;

public record UnlockUserCommand(Guid UserId) : ICommand;