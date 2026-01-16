using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

public class PutWelcomeSettings(DataContext dataContext)
    : EndpointWithMapper<WelcomeSettingsRequest, WelcomeSettingsMapper>
{
    public override void Configure()
    {
        Put("/settings/welcome");
        Roles(RoleNames.Admin);
        Description(builder => builder.Produces(204)
                                      .WithTags("Settings"));
    }

    public override async Task HandleAsync(WelcomeSettingsRequest request, CancellationToken ct)
    {
        var setting = await dataContext.Settings
                                       .FirstOrDefaultAsync(setting => setting.Key == SettingKey.WelcomeContent, ct);
        var mappedSetting = Map.ToEntity(request);
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
        await SendNoContentAsync(ct);
    }
}