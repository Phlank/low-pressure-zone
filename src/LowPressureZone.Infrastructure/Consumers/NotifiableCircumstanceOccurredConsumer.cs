using LowPressureZone.Infrastructure.Emails;
using LowPressureZone.Infrastructure.Events;
using Wolverine.Attributes;

namespace LowPressureZone.Infrastructure.Consumers;

[WolverineHandler]
public class NotifiableCircumstanceOccurredConsumer(IEmailService email)
{
    public async Task ConsumeAsync(NotifiableCircumstanceOccurred @event, CancellationToken ct) =>
        await email.SendAdminException(@event.Exception, @event.Process, @event.AdditionalContext);
}