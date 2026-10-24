namespace LowPressureZone.Domain.Settings;

public class WelcomeSettings
{
    public ICollection<TabContent> Tabs { get; set; } = [];
}