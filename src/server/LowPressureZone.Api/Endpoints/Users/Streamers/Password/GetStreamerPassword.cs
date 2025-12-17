using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Api.Endpoints.Users.Streamers.Password;

public class GetStreamerPassword(UserManager<AppUser> userManager, IAzuraCastClient client)
    : EndpointWithoutRequest<StreamerPasswordResponse>
{
    public override void Configure()
        => Get("/users/streamers/password");

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
        if (!generateResult.IsSuccess) ThrowError(generateResult.Error);

        StreamerPasswordResponse response = new()
        {
            Password = generateResult.Value
        };
        await SendOkAsync(response, ct);
    }
}