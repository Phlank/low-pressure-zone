using FastEndpoints;
using LowPressureZone.Identity.Application.Commands.EnableStreamer;
using Quartz;

namespace LowPressureZone.Identity.Application.Jobs;

[RegisterService<IJob<EnableStreamerCommand>>(LifeTime.Scoped)]
public class EnableStreamerJob(ICommandHandler<EnableStreamerCommand> commandHandler) : IJob<EnableStreamerCommand>
{
    public async ValueTask Execute(IJobExecutionContext context,
                                   EnableStreamerCommand input,
                                   CancellationToken cancellationToken = new()) =>
        await commandHandler.ExecuteAsync(input, cancellationToken);
    
    public static string JobKey(Guid userId) => $"{nameof(EnableStreamerJob)}-{userId}";
}