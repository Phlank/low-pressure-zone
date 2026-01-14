using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

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