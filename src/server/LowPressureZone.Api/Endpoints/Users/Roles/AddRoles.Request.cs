namespace LowPressureZone.Api.Endpoints.Users.Roles
{
    public class AddRolesRequest
    {
        public string Id { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}