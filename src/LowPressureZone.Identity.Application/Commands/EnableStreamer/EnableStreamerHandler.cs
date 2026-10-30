using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Identity.Application.Commands.EnableStreamer;

[RegisterService<ICommandHandler<EnableStreamerCommand>>(LifeTime.Scoped)]
public class EnableStreamerHandler(IdentityContext identityContext, IAzuraCastClient client) : ICommandHandler<EnableStreamerCommand>
{
    public async Task ExecuteAsync(EnableStreamerCommand command, CancellationToken ct)
    {
        var streamerId = await identityContext.Users
                                              .AsNoTracking()
                                              .Where(user => user.Id == command.UserId)
                                              .Select(user => user.StreamerId)
                                              .FirstOrDefaultAsync(ct);
        if (!streamerId.HasValue) return;
        await client.EnableStreamerAsync(streamerId.Value);
    }
}