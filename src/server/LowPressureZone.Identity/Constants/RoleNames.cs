using System.Collections.Immutable;

namespace LowPressureZone.Identity.Constants;

public static class RoleNames
{
    public const string ADMIN = "Admin";
    public const string PERFORMER = "Performer";
    public const string LISTENER = "Listener";

    public static ImmutableArray<string> AllRoles = [
        ADMIN, PERFORMER, LISTENER
    ];
}
