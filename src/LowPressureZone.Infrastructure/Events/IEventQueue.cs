using LowPressureZone.Core.Domain;

namespace LowPressureZone.Infrastructure.Events;

public interface IEventQueue
{
    void Enqueue(IEvent @event);
    void Enqueue(List<IEvent> events);
    Task FlushAsync(CancellationToken cancellationToken = default);
}