using LowPressureZone.Identity.Constants;

namespace LowPressureZone.Api.Constants;

internal static class Errors
{
    public const string Required = "Required";
    public const string Prohibited = "Prohibited";

    // Mapper errors
    public const string NoAuthorizedUserForMap = "Cannot map request to domain entity without an authorized user";

    // Entity errors
    public const string Unique = "Already in use";
    public const string EntityNotLinked = "Not linked to user";
    public const string DoesNotExist = "Does not exist";

    // String errors
    public const string InvalidUrl = "Invalid URL";
    public const string TimeInPast = "Cannot be in the past";

    // Timeslot errors
    public const string OverlapsOtherTimeslot = "Overlaps other timeslot";
    public const string OutOfScheduleRange = "Exceeds schedule";

    // Schedule errors
    public const string OverlapsOtherSchedule = "Overlaps other schedule";
    public const string ExcludesTimeslots = "Excludes timeslots";

    // User-event errors
    public const string InvalidEmail = "Invalid email";
    public const string EmailAlreadyInvited = "Already invited";
    public const string PasswordNumber = "Requires number";
    public const string PasswordUppercase = "Requires uppercase";
    public const string PasswordLowercase = "Requires lowercase";
    public const string PasswordSymbol = "Requires symbol";
    public const string ExpiredToken = "Your user registration link has expired. Please request a new one.";

    // User errors
    public static string InvalidRole => $"Allowed roles are {string.Join(" | ", RoleNames.AllRoles)}";

    // Other errors
    public static string NotEqual(string fieldName) => $"Cannot equal ${fieldName}";
    public static string MinLength(int value) => $"Minimum {value} characters";
    public static string MaxLength(int value) => $"Maximum {value} characters";

    // Time errors
    public static string MinDuration(int hours) => $"Minimum duration is {hours}h";
    public static string MaxDuration(int hours) => $"Maximum duration is {hours}h";
    
    // File errors
    public const string InvalidFileType = "Invalid file type";

    public static string UsernameInvalidCharacters(IEnumerable<string> characters) =>
        $"Invalid characters: {string.Join(' ', characters)}";
}