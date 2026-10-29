using FastEndpoints;
using LowPressureZone.Data;
using LowPressureZone.Identity;
using LowPressureZone.Infrastructure.Emails;
using LowPressureZone.Scheduling.Application.Commands.Schedules.SendReminder;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LowPressureZone.Scheduling.Application.Jobs;

[RegisterService<IJob<SendReminderCommand>>(LifeTime.Scoped)]
public class SendSlotReminderJob(ICommandHandler<SendReminderCommand> commandHandler)
    : IJob<SendReminderCommand>
{
    public async ValueTask Execute(IJobExecutionContext context,
                                   SendReminderCommand input,
                                   CancellationToken cancellationToken = new()) =>
        await commandHandler.ExecuteAsync(input, cancellationToken);

    public static string JobName(Guid slotId) => $"{nameof(SendSlotReminderJob)}-{slotId}";
}