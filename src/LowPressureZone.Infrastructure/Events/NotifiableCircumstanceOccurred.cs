namespace LowPressureZone.Infrastructure.Events;

public record NotifiableCircumstanceOccurred(
    string Process,
    string? AdditionalContext = null,
    Exception? Exception = null);