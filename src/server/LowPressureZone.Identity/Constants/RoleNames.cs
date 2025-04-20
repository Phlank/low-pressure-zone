namespace LowPressureZone.Identity.Constants;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Organizer = "Organizer"; // Transformed onto user

    public static string[] AllRoles => [Admin, Organizer];
    public static string[] DatabaseRoles => [Admin];
}
