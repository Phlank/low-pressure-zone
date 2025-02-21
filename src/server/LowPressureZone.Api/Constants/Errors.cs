namespace LowPressureZone.Api.Constants;

// These get mapped onto field labels, so they should remain as short as possible while being descriptive enough to solve the issues
public static class Errors
{
    public const string Required = "Required";
    public const string Unique = "Already in use";
    public const string InvalidUrl = "Invalid URL";
    public static string MinLength(int value) => $"Minimum {value} characters";

    // User errors
    public const string InvalidEmail = "Invalid email";
    public const string EmailAlreadyInvited = "Already invited";
    public const string PasswordNumber = "Requires number";
    public const string PasswordUppeercase = "Requires uppercase letter";
    public const string PasswordLowercase = "Requires lowercase letter";
    public const string PasswordSymbol = "Requires symbol";
}
