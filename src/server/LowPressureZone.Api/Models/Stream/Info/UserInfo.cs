namespace LowPressureZone.Api.Models.Stream.Info;

public class UserInfo
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? DisplayName { get; set; }
}
