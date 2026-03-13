using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Identity.Constants;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.PrivacyPolicy;

public class PutPrivacyPolicy(DataContext dataContext) : EndpointWithMapper<PrivacyPolicySettingsRequest, PrivacyPolicySettingsMapper>
{
    public override void Configure()
    {
        Put("/settings/privacypolicy");
        Description(builder => builder.WithTags("Settings"));
        Roles(RoleNames.Admin);
    }

    public override async Task HandleAsync(PrivacyPolicySettingsRequest req, CancellationToken ct)
    {
        var mappedSetting = Map.ToEntity(req);
        var setting = await dataContext.Settings
                                                 .Where(setting => setting.Key == SettingKey.PrivacyPolicy)
                                                 .FirstOrDefaultAsync(ct);
        if (setting is null)
        {
            dataContext.Add(mappedSetting);
        }
        else
        {
            setting.Value = mappedSetting.Value;
            setting.LastModifiedDate = DateTime.UtcNow;
        }
        
        await dataContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}