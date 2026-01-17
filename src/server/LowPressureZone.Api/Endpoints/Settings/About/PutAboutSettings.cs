using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.About;

public sealed class PutAboutSettings(DataContext dataContext)
    : EndpointWithMapper<AboutSettingsRequest, AboutSettingsMapper>
{
    public override void Configure()
    {
        Put("/settings/about");
        Roles(RoleNames.Admin);
        Description(b => b.WithTags("Settings")
                          .Produces(204));
    }

    public override async Task HandleAsync(AboutSettingsRequest req, CancellationToken ct)
    {
        var setting = await dataContext.Settings
                                       .FirstOrDefaultAsync(setting => setting.Key == SettingKey.AboutContent, ct);
        var mappedSetting = Map.ToEntity(req);
        if (setting is null)
        {
            dataContext.Settings.Add(mappedSetting);
        }
        else
        {
            setting.Value = mappedSetting.Value;
            setting.LastModifiedDate = mappedSetting.LastModifiedDate;
        }

        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}