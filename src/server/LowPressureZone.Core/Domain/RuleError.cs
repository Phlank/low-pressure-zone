namespace LowPressureZone.Core.Domain;

public record RuleError(string Message, string? Field = null);