namespace LowPressureZone.Identity.Constants;

public class RoleNames
{
    public const string Admin = "Admin";
    public const string Organizer = "Organizer";
    public const string Performer = "Performer";

    public static string[] All => [Admin, Organizer, Performer];
}
