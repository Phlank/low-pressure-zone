namespace LowPressureZone.Core.Domain;

public interface IAsyncRule<T> where T : Entity
{
    public Task<bool> IsBrokenAsync(T entity, CancellationToken ct = default);
    public RuleError Error { get; }
    
    public string? ToString() => $"{Error.Field}: {Error.Message}";
}