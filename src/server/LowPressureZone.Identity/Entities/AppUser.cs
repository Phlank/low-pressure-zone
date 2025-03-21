using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace LowPressureZone.Identity.Entities;

public class AppUser : IdentityUser<Guid>
{
    public const int DisplayNameMaxLength = 50;
    public AppUser() : base() { }

    [MaxLength(DisplayNameMaxLength)]
    public required string DisplayName { get; set; }

    public Invitation<Guid, AppUser>? Invitation { get; set; }
}
