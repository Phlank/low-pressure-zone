using FluentValidation.Results;

namespace LowPressureZone.Api.Extensions;

public static class StringExtensions
{
    public static bool StartsWithAny(
        this string value,
        IEnumerable<string> itemsToCompare,
        StringComparison comparison = StringComparison.InvariantCulture) =>
        itemsToCompare.Any(item => value.StartsWith(item, comparison));

    public static ValidationFailure ToValidationFailure(this string error, string? propertyName) =>
        new(propertyName, error);

    public static IEnumerable<ValidationFailure> ToValidationFailures(
        this string error,
        string? propertyName = null) => [new(propertyName, error)];
}