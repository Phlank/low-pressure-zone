using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.Link;

public partial class PostLinkStreamer(IAzuraCastClient client, UserManager<AppUser> userManager)
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
            await Send.NotFoundAsync(ct);
            return;
        }

        var createResult = await userManager.LinkToNewStreamer(user, client);
        if (createResult.IsSuccess)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        FailedToCreateNewStreamer(Logger, nameof(PostLinkStreamers), user.UserName ?? "null", createResult.Error);

        var linkResult = await userManager.LinkToExistingStreamer(user, client);
        if (linkResult.IsSuccess)
        {
            await Send.NoContentAsync(ct);
            return;
        }

        LogFailedToLinkExistingStreamer(Logger, nameof(PostLinkStreamers), user.UserName ?? "null", linkResult.Error);

        ThrowError($"Failed to create or link streamer for user. Check {nameof(PostLinkStreamer)} logs for more information.");
    }

    [LoggerMessage(LogLevel.Warning, "{source}: Failed to create new streamer for {userName}: {createResultError}")]
    static partial void FailedToCreateNewStreamer(
        ILogger logger,
        string source,
        string userName,
        string createResultError);

    [LoggerMessage(LogLevel.Warning, "{source}: Failed to link existing streamer to {userName}: {linkResultError}")]
    static partial void LogFailedToLinkExistingStreamer(
        ILogger logger,
        string source,
        string userName,
        string linkResultError);
}