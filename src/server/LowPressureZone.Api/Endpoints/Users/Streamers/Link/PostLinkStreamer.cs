using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.Link;

public class PostLinkStreamer(AzuraCastClient client, UserManager<AppUser> userManager)
    : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Post("/users/streamers/link/{userId}");
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Route<Guid>("userId");
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var createResult = await userManager.LinkToNewStreamer(user, client);
        if (createResult.IsSuccess)
        {
            await SendNoContentAsync(ct);
            return;
        }

        Logger.LogWarning("{Controller}: Failed to create new streamer for {UserName}: {CreateResultError}",
                          nameof(PostLinkStreamers), user.UserName, createResult.Error);

        var linkResult = await userManager.LinkToExistingStreamer(user, client);
        if (linkResult.IsSuccess)
        {
            await SendNoContentAsync(ct);
            return;
        }

        Logger.LogWarning("{Controller}: Failed to link existing streamer to {UserName}: {LinkResultError}",
                          nameof(PostLinkStreamers), user.UserName, linkResult.Error);

        ThrowError($"Failed to create or link streamer for user. Check {nameof(PostLinkStreamer)} logs for more information.");
    }
}
