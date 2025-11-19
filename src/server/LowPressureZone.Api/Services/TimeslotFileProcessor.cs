using FluentValidation.Results;
using LowPressureZone.Api.Endpoints.Schedules.Timeslots;
using LowPressureZone.Api.Extensions;
using LowPressureZone.Core;
using Shouldly;

namespace LowPressureZone.Api.Services;

public sealed class TimeslotFileProcessor(FormFileSaver fileSaver, MediaAnalyzer mediaAnalyzer)
{
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
            _ = await fileSaver.DeleteSavedFormFileAsync(saveResult.Value);
            return Result.Err<string>(analysisResult.Error.ToValidationFailures(nameof(request.File)));
        }

        var analysis = analysisResult.Value;
        var analysisValidationFailures = TimeslotRequestValidator.ValidateMediaAnalysis(request, analysis);
        if (analysisValidationFailures.Count != 0)
        {
            _ = await fileSaver.DeleteSavedFormFileAsync(saveResult.Value);
            return Result.Err<string>(analysisValidationFailures);
        }

        return Result.Ok<string, IEnumerable<ValidationFailure>>(saveResult.Value);
    }
}