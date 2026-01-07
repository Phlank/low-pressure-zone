using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

public sealed class GetWelcomeSettings(DataContext dataContext)
    : EndpointWithoutRequest<WelcomeSettingsResponse, WelcomeSettingsMapper>
{
    public override void Configure()
    {
        Get("/settings/welcome");
        Roles(RoleNames.Performer, RoleNames.Organizer, RoleNames.Admin);
        Description(b => b.WithTags("Settings", "Welcome")
                          .Produces(200));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var setting = await dataContext.Settings
                                       .FirstOrDefaultAsync(setting => setting.Key == SettingKey.WelcomeContent, ct);
        var response = Map.ToResponse(setting);
        await SendOkAsync(response, ct);
    }
}