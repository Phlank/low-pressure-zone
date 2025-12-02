using System.Security.Cryptography;
using System.Text;

namespace LowPressureZone.Api.Utilities;

public static class PasswordGenerator
{
    private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";

    public static string Generate(int length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive value.");

        var result = new StringBuilder(length);
        var data = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(data);
        }

        for (var i = 0; i < length; i++)
        {
            var charIndex = data[i] % AllowedChars.Length;
            result.Append(AllowedChars[charIndex]);
        }

        return result.ToString();
    }
}