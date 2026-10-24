namespace LowPressureZone.Core.Domain;

public interface IRule
{
    bool IsBroken();
    RuleError Error { get; }
    
    public string ToString() => $"{Error.Field}: {Error.Message}";
}