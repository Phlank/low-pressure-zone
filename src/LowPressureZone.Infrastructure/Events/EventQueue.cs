using FastEndpoints;

namespace LowPressureZone.Infrastructure.Events;

[RegisterService<IEventQueue>(LifeTime.Scoped)]
public class EventQueue : IEventQueue
{
    private readonly HashSet<IEvent> _events = [];
    
    public void Enqueue(IEvent @event) => _events.Add(@event);
    public void Enqueue(List<IEvent> events) => events.ForEach(@event => _events.Add(@event));

    public async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        foreach (var @event in _events)
        {
            await @event.PublishAsync(Mode.WaitForAll, cancellationToken);
        }
    }
}