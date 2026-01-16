namespace LowPressureZone.Domain.Entities.Settings;

public class WelcomeSettings
{
    public ICollection<TabContent> Tabs { get; set; } = [];
}