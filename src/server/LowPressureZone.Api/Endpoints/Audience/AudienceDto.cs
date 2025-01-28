namespace LowPressureZone.Api.Endpoints.Audience;

public class AudienceDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
