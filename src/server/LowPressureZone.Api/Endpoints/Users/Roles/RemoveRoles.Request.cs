namespace LowPressureZone.Api.Endpoints.Users.Roles;

public class RemoveRolesRequest
{
    public string Id { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}