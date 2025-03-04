namespace LowPressureZone.Api.Endpoints.Users;

public class GetUsersRequest
{
    public IEnumerable<string>? Roles { get; set; }
}