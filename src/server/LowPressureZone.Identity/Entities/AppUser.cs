using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Identity.Entities;

public class AppUser : IdentityUser<Guid>
{
    public AppUser() : base() { }
}
