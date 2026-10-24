using FastEndpoints;
using FFMpegCore;
using FFMpegCore.Exceptions;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.Audio;

[RegisterService<MediaAnalyzer>(LifeTime.Singleton)]
public sealed class MediaAnalyzer(ILogger<MediaAnalyzer> logger)
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
            logger.LogError(ex,
                            $"{nameof(FFMpegException)} while analyzing media file at {{FilePath}}: {{ErrorMessage}}",
                            filePath,
                            ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                            "Exception while analyzing media file at {FilePath}: {ErrorMessage}",
                            filePath,
                            ex.Message);
        }

        return Result.Err<IMediaAnalysis>("Failed to analyze media file.");
    }
}