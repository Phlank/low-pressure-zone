using FastEndpoints;
using LowPressureZone.Domain;
using LowPressureZone.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LowPressureZone.Api.Endpoints.Settings.PrivacyPolicy;

public class GetPrivacyPolicy(DataContext dataContext)
    : EndpointWithoutRequest<PrivacyPolicySettingsResponse, PrivacyPolicySettingsMapper>
{
    public override void Configure()
    {
        Get("/settings/privacypolicy");
        Description(builder => builder.WithTags("Settings"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var setting = await dataContext.Settings
                                       .Where(setting => setting.Key == SettingKey.PrivacyPolicy)
                                       .FirstOrDefaultAsync(ct);
        
        var response = Map.FromEntity(setting);
        await Send.OkAsync(response, ct);
    }
}