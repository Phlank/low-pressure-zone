namespace LowPressureZone.Api.Services.StreamStatus;

public interface IStreamStatusService : IHostedService
{
    Models.Stream.StreamStatus? Status { get; }
    bool IsStarted { get; }
}