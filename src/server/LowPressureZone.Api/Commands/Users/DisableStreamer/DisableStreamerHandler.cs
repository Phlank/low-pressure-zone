using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Commands.Users.DisableStreamer;

public class DisableStreamerHandler(IdentityContext identityContext, IAzuraCastClient client)
    : ICommandHandler<DisableStreamerCommand>
{
    public async Task ExecuteAsync(DisableStreamerCommand command, CancellationToken ct)
    {
        var streamerId = await identityContext.Users
                                              .Where(user => user.Id == command.UserId)
                                              .Select(user => user.StreamerId)
                                              .FirstOrDefaultAsync(ct);
        if (!streamerId.HasValue) return;
        await client.DisableStreamerAsync(streamerId.Value);
    }
}