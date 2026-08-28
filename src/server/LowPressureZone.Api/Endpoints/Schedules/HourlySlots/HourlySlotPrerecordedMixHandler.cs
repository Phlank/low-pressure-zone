using System.Diagnostics;
using FastEndpoints;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using LowPressureZone.Domain.ScheduleAggregate;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity;
using LowPressureZone.Domain.ScheduleAggregate.HourlySlotEntity.PrerecordedMixObject;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

[RegisterService<HourlySlotPrerecordedMixHandler>(LifeTime.Scoped)]
public class HourlySlotPrerecordedMixHandler(
    PrerecordedMixFileProcessor fileProcessor,
    PrerecordedMixCleanupService cleanupService)
{
    public async Task<DomainResult<PrerecordedMix>> AddNewMixFile<TRequest, TResponse>(
        Endpoint<TRequest, TResponse> endpoint,
        Schedule schedule,
        HourlySlotRequest request,
        CancellationToken ct) where TRequest : notnull
    {
        Debug.Assert(request.File is not null);

        var processResult = await fileProcessor.ProcessRequestFileToMp3Async(request, ct);
        if (processResult.IsError)
        {
            endpoint.ValidationFailures.AddRange(processResult.Error);
            endpoint.ThrowIfAnyErrors();
        }

        var enqueueResult = await fileProcessor.EnqueuePrerecordedMixAsync(request,
                                                                           schedule.TimeRange.StartsAt,
                                                                           processResult.Value,
                                                                           ct);
        if (enqueueResult.IsError)
        {
            endpoint.ThrowError(enqueueResult.Error);
        }

        return PrerecordedMix.Create(true, Path.GetFileName(request.File.FileName), enqueueResult.Value);
    }

    public async Task<DomainResult<PrerecordedMix>> ReplaceMixFile<TRequest, TResponse>(
        Endpoint<TRequest, TResponse> endpoint,
        HourlySlot slot,
        HourlySlotRequest request,
        CancellationToken ct) where TRequest : notnull
    {
        Debug.Assert(request.File is not null);

        var result = await fileProcessor.UpdateEnqueuedPrerecordedMixAsync(slot.Id, request, ct);
        if (result.IsError)
        {
            endpoint.ValidationFailures.AddRange(result.Error);
            endpoint.ThrowIfAnyErrors();
        }

        return PrerecordedMix.Create(true, Path.GetFileName(request.File.FileName), result.Value);
    }

    public async Task<DomainResult<NoValue>> DeleteMixFile<TRequest, TResponse>(
        Endpoint<TRequest, TResponse> endpoint,
        HourlySlot slot) where TRequest : notnull
    {
        Debug.Assert(slot.Prerecord.AzuraCastMediaId.HasValue);

        var deleteResult = await cleanupService.DeleteEnqueuedPrerecordedMixAsync(slot.Prerecord.AzuraCastMediaId!.Value);
        if (deleteResult.IsError)
        {
            endpoint.ThrowError(deleteResult.Error);
        }

        return DomainResult.Ok();
    }
}