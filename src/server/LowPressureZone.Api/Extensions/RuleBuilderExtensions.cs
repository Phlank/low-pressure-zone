using FluentValidation;

namespace LowPressureZone.Api.Extensions;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> AbsoluteHttpUri<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(e => Uri.IsWellFormedUriString(e, UriKind.Absolute)).WithMessage("Invalid URL.").Must(e => e.StartsWith("https://") || e.StartsWith("http://")).WithMessage("Invalid URL");
    }
}
