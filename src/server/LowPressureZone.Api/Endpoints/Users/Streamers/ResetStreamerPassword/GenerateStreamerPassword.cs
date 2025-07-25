using FastEndpoints;
using LowPressureZone.Api.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.ResetStreamerPassword;

public class GenerateStreamerPassword(UserManager<AppUser> userManager, AzuraCastClient client)
    : EndpointWithoutRequest<GenerateStreamerPasswordResponse>
{
    public override void Configure()
        => Post("/users/streamers/generatepassword");

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.Identity?.Name is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var user = await userManager.FindByNameAsync(User.Identity.Name);
        if (user is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var generateResult = await userManager.GenerateStreamerPassword(user, client);
        if (!generateResult.IsSuccess) ThrowError(generateResult.Error ?? "Unable to generate new password");

        GenerateStreamerPasswordResponse response = new()
        {
            Password = generateResult.Value!
        };
        await SendOkAsync(response, ct);
    }
}