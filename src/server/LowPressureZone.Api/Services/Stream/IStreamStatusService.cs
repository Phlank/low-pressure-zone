using LowPressureZone.Api.Models.Stream;

namespace LowPressureZone.Api.Services.Stream;

public interface IStreamStatusService
{
    StreamStatus? Status { get; }
    bool IsStarted { get; }
}
