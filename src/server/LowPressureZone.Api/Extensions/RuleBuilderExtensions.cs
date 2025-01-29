using FluentValidation;

namespace LowPressureZone.Api.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> AbsoluteHttpsUri<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(e => Uri.IsWellFormedUriString(e, UriKind.Absolute)).WithMessage("Invalid URL.").Must(e => e.StartsWith("https://")).WithMessage("Invalid URL");
    }
}
