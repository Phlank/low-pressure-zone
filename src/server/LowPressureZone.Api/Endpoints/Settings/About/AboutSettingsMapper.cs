using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Settings.About;

public class AboutSettingsMapper : IRequestMapper, IResponseMapper
{
    public Setting ToEntity(AboutSettingsRequest req) =>
        new()
        {
            Key = SettingKey.AboutContent,
            Value = JsonSerializer.Serialize(req)
        };

    public AboutSettingsResponse ToResponse(Setting? setting)
    {
        if (setting is not null)
        {
            var response = JsonSerializer.Deserialize<AboutSettingsResponse>(setting.Value);
            if (response is not null)
                return response;
        }

        return new AboutSettingsResponse
        {
            TopText = "",
            BottomText = ""
        };
    }
}