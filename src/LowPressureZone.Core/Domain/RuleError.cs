using FluentValidation.Results;

namespace LowPressureZone.Core.Domain;

public record RuleError(string Message, string? Field = null);

public static class RuleErrorExtensions
{
    public static ValidationFailure ToValidationFailure(this RuleError error) => new(error.Field, error.Message);
}