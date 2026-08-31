using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Domain.Enums;
using LowPressureZone.Domain.Settings;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

[RegisterService<WelcomeSettingsMapper>(LifeTime.Singleton)]
public sealed class WelcomeSettingsMapper : IRequestMapper, IResponseMapper
{
    private static readonly WelcomeSettingsResponse DefaultResponse = new()
    {
        Tabs = []
    };

    public Setting ToEntity(WelcomeSettingsRequest req) => new()
    {
        Key = SettingKey.WelcomeContent,
        Value = JsonSerializer.Serialize(req)
    };
    
    public WelcomeSettingsResponse ToResponse(Setting? setting)
    {
        if (setting is null)
            return DefaultResponse;

        try
        {
            var response = JsonSerializer.Deserialize<WelcomeSettingsResponse>(setting.Value);
            if (response is not null)
                return response;

            return DefaultResponse;
        }
        catch (Exception)
        {
            return DefaultResponse;
        }
    }
}