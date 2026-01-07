using LowPressureZone.Api.Models;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

public sealed class WelcomeSettingsResponse
{
    public List<TabContent> Tabs { get; set; } = [];
}