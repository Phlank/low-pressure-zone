using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Commands.Users.EnableStreamer;

public class EnableStreamerHandler(IdentityContext identityContext, IAzuraCastClient client) : ICommandHandler<EnableStreamerCommand>
{
    public async Task ExecuteAsync(EnableStreamerCommand command, CancellationToken ct)
    {
        var streamerId = await identityContext.Users
                                              .Where(user => user.Id == command.UserId)
                                              .Select(user => user.StreamerId)
                                              .FirstOrDefaultAsync(ct);
        if (!streamerId.HasValue) return;
        await client.EnableStreamerAsync(streamerId.Value);
    }
}