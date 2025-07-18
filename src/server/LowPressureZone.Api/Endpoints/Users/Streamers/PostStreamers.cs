﻿using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Constants;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Users.Streamers;

public class PostStreamers(AzuraCastClient client, UserManager<AppUser> userManager) : EndpointWithoutRequest<EmptyResponse>
{
    public override void Configure()
    {
        Post("/users/streamers");
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
            Logger.LogWarning("{Controller}: Failed to create new streamer for {UserName}: {CreateResultError}", nameof(PostStreamers), user.UserName, createResult.Error);

            var linkResult = await userManager.LinkToExistingStreamer(user, client);
            if (linkResult.IsSuccess) continue;
            Logger.LogWarning("{Controller}: Failed to link existing streamer to {UserName}: {LinkResultError}", nameof(PostStreamers), user.UserName, linkResult.Error);

            fails++;
        }

        if (fails == usersWithoutStreamer.Count && fails != 0) ThrowError($"Failed to create or link streamer for {fails} of  {usersWithoutStreamer.Count} users. Check {nameof(PostStreamers)} logs for more information.");
        await SendNoContentAsync(ct);
    }
}
