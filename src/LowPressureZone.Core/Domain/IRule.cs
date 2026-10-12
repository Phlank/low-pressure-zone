namespace LowPressureZone.Core.Domain;

public interface IRule
{
    public bool IsBroken();
    public RuleError Error { get; }
}