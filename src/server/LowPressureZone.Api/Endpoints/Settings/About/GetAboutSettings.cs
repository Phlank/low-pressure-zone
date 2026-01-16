using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.About;

public sealed class GetAboutSettings(DataContext dataContext)
    : EndpointWithoutRequest<AboutSettingsResponse, AboutSettingsMapper>
{
    public override void Configure()
    {
        Get("/settings/about");
        AllowAnonymous();
        Description(b => b.WithTags("Settings")
                          .Produces<AboutSettingsResponse>());
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var setting = await dataContext.Settings
                                       .Where(setting => setting.Key == SettingKey.AboutContent)
                                       .FirstOrDefaultAsync(ct);
        var mappedResponse = Map.ToResponse(setting);
        await SendOkAsync(mappedResponse, ct);
    }
}