using System.Text.Json;
using FastEndpoints;
using LowPressureZone.Domain.Entities.Settings;
using LowPressureZone.Domain.Enums;

namespace LowPressureZone.Api.Endpoints.Settings.About;

public sealed class AboutSettingsMapper(ILogger<AboutSettingsMapper> logger) : IRequestMapper, IResponseMapper
{
    public Setting ToEntity(AboutSettingsRequest req) =>
        new()
        {
            Key = SettingKey.AboutContent,
            Value = JsonSerializer.Serialize(req)
        };

    public AboutSettingsResponse ToResponse(Setting? setting)
    {
        if (setting is null)
            return DefaultResponse;
        
        try
        {
            var response = JsonSerializer.Deserialize<AboutSettingsResponse>(setting.Value);
            if (response is not null)
                return response;

            return DefaultResponse;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize AboutSettings value");
            return DefaultResponse;
        }
    }
    
    private static readonly AboutSettingsResponse DefaultResponse = new()
    {
        TopText = "",
        BottomText = ""
    };
}