using System.Text.RegularExpressions;
using FluentValidation;
using LowPressureZone.Api.Constants;

namespace LowPressureZone.Api.Extensions;

public static class RuleBuilderExtensions
{
    private static readonly Regex InvalidUsernameCharactersRegex = new Regex("[^A-Za-z0-9-._@+]", RegexOptions.Compiled);

    private static readonly Regex PasswordNumberRegex = new Regex("[0-9]", RegexOptions.Compiled);
    private static readonly Regex PasswordSpecialCharacterRegex = new Regex("[^A-Za-z0-9]", RegexOptions.Compiled);
    private static readonly Regex PasswordLowercaseRegex = new Regex("[a-z]", RegexOptions.Compiled);
    private static readonly Regex PasswordUppercaseRegex = new Regex("[A-Z]", RegexOptions.Compiled);
    public static IRuleBuilder<T, string> AbsoluteHttpUri<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.Must(e => Uri.IsWellFormedUriString(e, UriKind.Absolute)).WithMessage(Errors.InvalidUrl).Must(e => e.StartsWith("https://", StringComparison.InvariantCulture) || e.StartsWith("http://", StringComparison.InvariantCulture)).WithMessage(Errors.InvalidUrl);
    public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.NotEmpty()
                      .WithMessage(Errors.Required)
                      .MaximumLength(256)
                      .WithMessage(Errors.MaxLength(256))
                      .Custom((username, context) =>
                      {
                          var invalidCharacterMatches = InvalidUsernameCharactersRegex.Matches(username);
                          if (invalidCharacterMatches.Count > 0)
                          {
                              var invalidCharacters = invalidCharacterMatches.Select(match => match.Value).Distinct();
                              context.AddFailure(nameof(username), Errors.UsernameInvalidCharacters(invalidCharacters));
                          }
                      });
    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
        => ruleBuilder.MinimumLength(8).WithMessage(Errors.MinLength(8)).Custom(static (password, context) =>
        {
            if (!PasswordNumberRegex.IsMatch(password)) context.AddFailure(nameof(password), Errors.PasswordNumber);
            if (!PasswordSpecialCharacterRegex.IsMatch(password)) context.AddFailure(nameof(password), Errors.PasswordSymbol);
            if (!PasswordLowercaseRegex.IsMatch(password)) context.AddFailure(nameof(password), Errors.PasswordLowercase);
            if (!PasswordUppercaseRegex.IsMatch(password)) context.AddFailure(nameof(password), Errors.PasswordUppeercase);
        });
}
