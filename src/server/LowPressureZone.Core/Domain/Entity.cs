namespace LowPressureZone.Core.Domain;

public class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}