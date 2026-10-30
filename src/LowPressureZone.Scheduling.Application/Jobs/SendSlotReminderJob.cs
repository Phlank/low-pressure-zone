using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Emails;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LowPressureZone.Scheduling.Application.Jobs;

[RegisterService<IJob<SendSlotReminderCommand>>(LifeTime.Scoped)]
public class SendSlotReminderJob(ICommandHandler<SendSlotReminderCommand> commandHandler)
    : IJob<SendSlotReminderCommand>
{
    public async ValueTask Execute(IJobExecutionContext context,
                                   SendSlotReminderCommand input,
                                   CancellationToken cancellationToken = new()) =>
        await commandHandler.ExecuteAsync(input, cancellationToken);

    public static string JobName(Guid slotId) => $"{nameof(SendSlotReminderJob)}-{slotId}";
}