using FastEndpoints;
using FFMpegCore;
using FluentValidation.Results;
using LowPressureZone.Api.Endpoints.Schedules.HourlySlots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Api.Services.Audio;
using LowPressureZone.Core;
using LowPressureZone.Core.Domain;
using Shouldly;
using Results = LowPressureZone.Core.Results;

namespace LowPressureZone.Api.Services.Files;

[RegisterService<PrerecordedMixFileProcessor>(LifeTime.Scoped)]
public sealed class PrerecordedMixFileProcessor(
    FormFileSaver fileSaver,
    MediaAnalyzer mediaAnalyzer,
    Mp3Processor mp3Processor)
{
    private const int PrerecordedDurationMinutesTolerance = 2;

    public async Task<DomainResult<string>> ProcessPrerecordedMixFileToMp3Async(
        HourlySlotRequest request,
        CancellationToken ct = default)
    {
        request.File.ShouldNotBeNull();
        var saveResult = await fileSaver.SaveFormFileAsync(request.File, ct);
        if (saveResult.IsError)
            return DomainResult.Err<string>([new RuleError(saveResult.Error)]);

        var analysisResult = await mediaAnalyzer.AnalyzeAsync(saveResult.Value, ct);
        if (analysisResult.IsError)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return DomainResult.Err<string>([new RuleError(analysisResult.Error)]);
        }

        var analysis = analysisResult.Value;
        var analysisValidationFailures = ValidateMediaAnalysis(request, analysis);
        if (analysisValidationFailures.Count != 0)
        {
            _ = await fileSaver.DeleteFileAsync(saveResult.Value);
            return DomainResult.Err<string>([
                .. analysisValidationFailures.Select(failure => new RuleError(failure.ErrorMessage))
            ]);
        }

        var processResult = await ProcessToNewFile(analysis, saveResult.Value);
        _ = await fileSaver.DeleteFileAsync(saveResult.Value);

        if (processResult.IsError)
            return DomainResult.Err<string>([
                .. processResult.Error.Select(failure => new RuleError(failure.ErrorMessage))
            ]);

        return DomainResult.Ok(processResult.Value);
    }

    public static ICollection<ValidationFailure> ValidateMediaAnalysis(HourlySlotRequest request,
                                                                       IMediaAnalysis analysis)
    {
        request.File.ShouldNotBeNull();
        List<ValidationFailure> failures = [];
        var timeslotDuration = request.StartsAt.AddHours(request.Duration) - request.StartsAt;
        if (TimeSpan.FromMinutes(timeslotDuration.TotalMinutes - PrerecordedDurationMinutesTolerance) >
            analysis.Duration
            || TimeSpan.FromMinutes(timeslotDuration.TotalMinutes + PrerecordedDurationMinutesTolerance) <
            analysis.Duration)
            failures.Add(new ValidationFailure(nameof(request.File),
                                               "Media file duration does not match the specified timeslot duration. Ensure it is +/- 2 minutes from the timeslot duration."));

        failures.AddRange(AudioQualityValidator.ValidateAudioQuality(analysis,
                                                                     request.File.Length,
                                                                     nameof(request.File)));
        return failures;
    }

    private async Task<Result<string, IEnumerable<ValidationFailure>>> ProcessToNewFile(
        IMediaAnalysis analysis,
        string inputFilePath)
    {
        string outputFilePath;
        analysis.AudioStreams.ShouldHaveSingleItem();
        if (analysis.AudioStreams[0].CodecName == "mp3")
        {
            var stripResult = await mp3Processor.StripMp3MetadataAsync(inputFilePath);
            if (stripResult.IsError)
                return Results.Err<string>(stripResult.Error.ToValidationFailures(nameof(HourlySlotRequest.File)));

            outputFilePath = stripResult.Value;
        }
        else
        {
            var conversionResult = await mp3Processor.ConvertFileToMp3Async(inputFilePath);
            if (conversionResult.IsError)
                return Results.Err<string>(conversionResult.Error.ToValidationFailures(nameof(HourlySlotRequest.File)));

            outputFilePath = conversionResult.Value;
        }

        return Results.Ok<string, IEnumerable<ValidationFailure>>(outputFilePath);
    }
}