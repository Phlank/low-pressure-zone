using FastEndpoints;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Settings.PrivacyPolicy;

public sealed class PrivacyPolicySettingsMapper : IRequestMapper, IResponseMapper
{
    public Setting ToEntity(PrivacyPolicySettingsRequest request) =>
        new()
        {
            Key = SettingKey.PrivacyPolicy,
            Value = request.PrivacyPolicy
        };

    public PrivacyPolicySettingsResponse FromEntity(Setting? setting)
    {
        if (setting is null)
            return new PrivacyPolicySettingsResponse
            {
                PrivacyPolicy = ""
            };

        return new PrivacyPolicySettingsResponse
        {
            PrivacyPolicy = setting.Value
        };
    }
}