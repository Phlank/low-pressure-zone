using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.Link;

public partial class PostLinkStreamers(IAzuraCastClient client, UserManager<AppUser> userManager)
    : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Post("/users/streamers/link");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var usersWithoutStreamer = await userManager.Users
                                                    .Where(user => user.StreamerId == null
                                                                   && (user.Invitation == null
                                                                       || user.Invitation.IsRegistered))
                                                    .ToListAsync(ct);

        var fails = 0;
        foreach (var user in usersWithoutStreamer)
        {
            var createResult = await userManager.LinkToNewStreamer(user, client);
            if (createResult.IsSuccess) continue;
            LogFailedToCreateNewStreamer(Logger, nameof(PostLinkStreamers), user.UserName!, createResult.Error);

            var linkResult = await userManager.LinkToExistingStreamer(user, client);
            if (linkResult.IsSuccess) continue;
            LogFailedToLinkExistingStreamerToUser(Logger, nameof(PostLinkStreamers), user.UserName!, linkResult.Error);

            fails++;
        }

        if (fails == usersWithoutStreamer.Count && fails != 0)
            ThrowError($"Failed to create or link streamer for {fails} of  {usersWithoutStreamer.Count} users. Check {nameof(PostLinkStreamers)} logs for more information.");
        await SendNoContentAsync(ct);
    }

    [LoggerMessage(LogLevel.Warning, "{source}: Failed to create new streamer for {userName}: {createResultError}")]
    static partial void LogFailedToCreateNewStreamer(
        ILogger logger,
        string source,
        string userName,
        string createResultError);

    [LoggerMessage(LogLevel.Warning, "{source}: Failed to link existing streamer to {userName}: {linkResultError}")]
    static partial void LogFailedToLinkExistingStreamerToUser(
        ILogger logger,
        string source,
        string userName,
        string linkResultError);
}