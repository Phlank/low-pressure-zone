using FastEndpoints;

namespace LowPressureZone.Api.Commands.Users.Lock;

public record LockUserCommand(Guid UserId) : ICommand;