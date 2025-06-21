using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Services.Stream;

public interface IStreamStatusService : IHostedService
{
    StreamStatus? Status { get; }
    bool IsStarted { get; }
}
