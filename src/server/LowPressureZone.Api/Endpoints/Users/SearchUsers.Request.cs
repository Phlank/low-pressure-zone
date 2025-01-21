using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace LowPressureZone.Api.Endpoints.Users;

public class SearchUsersRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
}