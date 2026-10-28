using FastEndpoints;
using IEvent = LowPressureZone.Core.Domain.IEvent;

namespace LowPressureZone.Infrastructure.Events;

[RegisterService<IEventQueue>(LifeTime.Scoped)]
public class EventQueue(IEventBus eventBus) : IEventQueue
{
    private readonly HashSet<IEvent> _events = new();
    
    public void Enqueue(IEvent @event) => _events.Add(@event);
    public void Enqueue(List<IEvent> events) => events.ForEach(@event => _events.Add(@event));

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var @event in _events)
        {
            await eventBus.PublishAsync(@event, Mode.WaitForAll, cancellationToken);
        }
    }
}