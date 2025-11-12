using System.Text.Json;
using FFMpegCore;
using FFMpegCore.Exceptions;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services;

public sealed partial class MediaAnalyzer(ILogger<MediaAnalyzer> logger)
{
    public async Task<Result<IMediaAnalysis, string>> AnalyzeAsync(string filePath, CancellationToken ct)
    {
        try
        {
            var analysis = await FFProbe.AnalyseAsync(filePath, null, ct);
            return Result.Ok(analysis);
        }
        catch (FFMpegException ex)
        {
            LogAnalysisException(logger, filePath, ex.Message);
            return Result.Err<IMediaAnalysis>($"Failed to analyze media file: {ex.Message}");
        }
    }

    [LoggerMessage(LogLevel.Error, "FFMpegException while analyzing media file at {filePath}: {errorMessage}")]
    static partial void LogAnalysisException(ILogger<MediaAnalyzer> logger, string filePath, string errorMessage);
}