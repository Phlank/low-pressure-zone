namespace LowPressureZone.Api.Constants;

// These get mapped onto field labels, so they should remain as short as possible while being descriptive enough to solve the issues
public static class Errors
{
    public static string Required = "Required";
    public const string Unique = "Already in use";
    public const string InvalidUrl = "Invalid URL";
    public static string MinLength(int value) => $"Minimum {value} characters";

    // User errors
    public const string InvalidEmail = "Invalid email";
    public const string EmailAlreadyInvited = "Already invited";
    public static string UsernameInvalidCharacters(IEnumerable<string> characters) => $"Invalid characters: {string.Join(' ', characters)}";
    public const string PasswordNumber = "Requires number";
    public const string PasswordUppeercase = "Requires uppercase";
    public const string PasswordLowercase = "Requires lowercase";
    public const string PasswordSymbol = "Requires symbol";
}
