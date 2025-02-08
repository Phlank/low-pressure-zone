using System.Collections.Immutable;

namespace LowPressureZone.Identity.Constants;

public record RoleNames
{
    public const string Admin = "Admin";
    public const string Publicist = "Publicist";
    public const string Performer = "Performer";
    public const string Listener = "Listener";

    public static string[] AllRoles => [Admin, Publicist, Performer, Listener];
}
