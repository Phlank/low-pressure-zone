﻿using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.Password;

public class GetStreamerPassword(UserManager<AppUser> userManager, AzuraCastClient client)
    : EndpointWithoutRequest<StreamerPasswordResponse>
{
    public override void Configure()
        => Get("/users/streamers/password");

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.Identity?.Name is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var user = await userManager.FindByNameAsync(User.Identity.Name);
        if (user is null)
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var generateResult = await userManager.GenerateStreamerPassword(user, client);
        if (!generateResult.IsSuccess) ThrowError(generateResult.Error ?? "Unable to generate new password");

        StreamerPasswordResponse response = new()
        {
            Password = generateResult.Value
        };
        await Send.OkAsync(response, ct);
    }
}