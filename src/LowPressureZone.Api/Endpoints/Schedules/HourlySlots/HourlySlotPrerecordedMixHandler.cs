using System.Diagnostics;
using FastEndpoints;
using LowPressureZone.Adapter.AzuraCast.Resolvers;
using LowPressureZone.Api.Services.AzuraCast;
using LowPressureZone.Api.Services.Files;
using LowPressureZone.Core.Domain;
using LowPressureZone.Infrastructure.Adapters;
using LowPressureZone.Infrastructure.Events;
using LowPressureZone.Infrastructure.Http;
using LowPressureZone.Infrastructure.Resolvers;
using LowPressureZone.Scheduling.Domain.ScheduleAggregate;
using LowPressureZone.Scheduling.Domain.UploadedMediaAggregate;

namespace LowPressureZone.Api.Endpoints.Schedules.HourlySlots;

[RegisterService<HourlySlotPrerecordedMixHandler>(LifeTime.Scoped)]
public class HourlySlotPrerecordedMixHandler(
    PrerecordedMixFileProcessor fileProcessor,
    IAzuraCastMediaPathResolver pathResolver,
    IS3Adapter s3,
    IEventBus eventBus)
{
    public async Task<DomainResult<UploadedMedia>> ValidateAndUploadMixFile(
        HourlySlotRequest request,
        string performerName,
        Schedule schedule,
        CancellationToken ct)
    {
        Debug.Assert(request.File is not null);

        var processResult = await fileProcessor.ProcessPrerecordedMixFileToMp3Async(request, ct);
        if (processResult.IsError)
        {
            return processResult.Switch<UploadedMedia>();
        }

        var localFilePath = processResult.Value;

        try
        {
            await using var fileStream = File.OpenRead(localFilePath);
            var uploadFileName = MediaFileNameResolver.Resolve(request.StartsAt, performerName, request.Subtitle);
            var uploadFilePath = pathResolver.ResolveFilePath(AzuraCastMediaDirectory.Prerecords, uploadFileName);
            var uploadResult = await s3.PutObjectAsync(fileStream, uploadFilePath, ct);
            fileStream.Close();
            File.Delete(localFilePath);
            if (uploadResult.HttpStatusCode.IsError())
            {
                await eventBus.PublishAsync(new NotifiableCircumstanceOccurred(nameof(ValidateAndUploadMixFile),
                                                                               $"Upload failed with status code {uploadResult.HttpStatusCode}"),
                                            cancellation: ct);
                return DomainResult.Err<UploadedMedia>(new RuleError("Upload to external provider failed",
                                                                     nameof(request.File)));
            }

            var uploadedMediaResult = UploadedMedia.Create(uploadFileName, uploadFilePath);
            return uploadedMediaResult;
        }
        catch (Exception ex)
        {
            await eventBus.PublishAsync(new NotifiableCircumstanceOccurred(nameof(ValidateAndUploadMixFile), null, ex),
                                        cancellation: ct);
            throw;
        }
    }
}