using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Identity.Entities;

public class AppUser : IdentityUser<Guid>
{
    public Invitation<Guid, AppUser>? Invitation { get; set; }

    public AppUser() : base() { }
}
