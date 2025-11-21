using System.Globalization;
using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Adapter.AzuraCast.Clients;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Models;
using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Api.Services.Audio;
using LowPressureZone.Core;
using LowPressureZone.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shouldly;

namespace LowPressureZone.Api.Services.Files;

public sealed class TimeslotFileProcessor(
    FormFileSaver fileSaver,
    MediaAnalyzer mediaAnalyzer,
    Mp3Processor mp3Processor,
    DataContext dataContext,
    AzuraCastClient azuraCastClient,
    IOptions<FileConfiguration> fileOptions)
{
    private readonly string _tempLocation = fileOptions.Value.TemporaryLocation;

    public async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessUploadedMediaFileAsync(
        TimeslotRequest request,
        CancellationToken ct = default)
    {
        request.File.ShouldNotBeNull();
        var saveResult = await fileSaver.SaveFormFileAsync(request.File, ct);
        if (saveResult.IsError)
            return Result.Err<string>(saveResult.Error.ToValidationFailures(nameof(request.File)));

        var analysisResult = await mediaAnalyzer.AnalyzeAsync(saveResult.Value, ct);
        if (analysisResult.IsError)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return Result.Err<string>(analysisResult.Error.ToValidationFailures(nameof(request.File)));
        }

        var analysis = analysisResult.Value;
        var analysisValidationFailures = TimeslotRequestValidator.ValidateMediaAnalysis(request, analysis);
        if (analysisValidationFailures.Count != 0)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return Result.Err<string>(analysisValidationFailures);
        }

        var newMetadata = await GetAudioMetadataAsync(request, ct);
        var fileName = GetUploadFileName(newMetadata.Artist, newMetadata.Title, request.StartsAt);
        var outputFilePath = Path.Combine(_tempLocation, fileName);

        var processResult = await ProcessToNewFile(analysis, saveResult.Value, outputFilePath);
        _ = await fileSaver.DeleteFileAsync(saveResult.Value);

        await using (var fileStream = File.OpenRead(outputFilePath))
        {
            var uploadMediaResult = await azuraCastClient.UploadMediaAsync(fileName, fileStream);
            if (uploadMediaResult.IsError)
            {
                return Result.Err<string>([
                    new ValidationFailure(nameof(request.File), "Failed to upload media to AzuraCast.")
                ]);
            }
        }

        _ = await fileSaver.DeleteFileAsync(outputFilePath);

        return processResult;
    }

    private async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessToNewFile(
        IMediaAnalysis analysis,
        string inputFilePath,
        string outputFilePath)
    {
        if (analysis.AudioStreams[0].CodecName == "mp3")
        {
            var stripResult = await mp3Processor.StripMp3MetadataAsync(inputFilePath, outputFilePath);
            if (stripResult.IsError)
                return Result.Err<string>(stripResult.Error.ToValidationFailures(nameof(TimeslotRequest.File)));
        }
        else
        {
            var conversionResult = await mp3Processor.ConvertFileToMp3Async(inputFilePath, outputFilePath);
            if (conversionResult.IsError)
                return Result.Err<string>(conversionResult.Error.ToValidationFailures(nameof(TimeslotRequest.File)));
        }

        return Result.Ok<string, IEnumerable<ValidationFailure>>(outputFilePath);
    }

    private async Task<SimpleAudioMetadata> GetAudioMetadataAsync(TimeslotRequest request, CancellationToken ct)
    {
        var performerName = await dataContext.Performers
                                             .Where(performer => performer.Id == request.PerformerId)
                                             .Select(performer => performer.Name)
                                             .FirstAsync(ct);
        return new SimpleAudioMetadata(request.Name, performerName);
    }

    private static string GetUploadFileName(string artist, string? title, DateTimeOffset start)
    {
        if (string.IsNullOrEmpty(title))
            return $"{artist} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";

        return $"{artist} - {title} - {start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)}.mp3";
    }
}