using FastEndpoints;

namespace LowPressureZone.Api.Commands.Users.Unlock;

public record UnlockUserCommand(Guid UserId) : ICommand;