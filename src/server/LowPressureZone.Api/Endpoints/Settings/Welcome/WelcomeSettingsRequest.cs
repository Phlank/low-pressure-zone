using LowPressureZone.Api.Models;

namespace LowPressureZone.Api.Endpoints.Settings.Welcome;

public sealed class WelcomeSettingsRequest
{
    public List<TabContent> Tabs { get; set; } = [];
}