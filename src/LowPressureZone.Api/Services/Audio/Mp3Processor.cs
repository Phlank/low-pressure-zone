using FastEndpoints;
using FFMpegCore;
using FFMpegCore.Enums;
using LowPressureZone.Core;
using Results = LowPressureZone.Core.Results;

namespace LowPressureZone.Api.Services.Audio;

[RegisterService<Mp3Processor>(LifeTime.Singleton)]
public sealed class Mp3Processor(ILogger<Mp3Processor> logger)
{
    public async Task<Result<string, string>> ConvertFileToMp3Async(string inputFilePath)
    {
        var outputFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        try
        {
            var conversionResult =
                await FFMpegArguments.FromFileInput(inputFilePath,
                                                    false)
                                     .OutputToFile(outputFilePath,
                                                   true,
                                                   options => options.WithAudioCodec(AudioCodec.LibMp3Lame)
                                                                     .WithAudioBitrate(320)
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously(true);

            if (conversionResult.Success)
                return Results.Ok(outputFilePath);

            logger.LogError("Unable to convert file to MP3 at {InputFilePath}", inputFilePath);
            return Results.Err<string>("Failed to process audio file.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                            "Unable to convert file to MP3 at {InputFilePath}: {ErrorMessage}",
                            inputFilePath,
                            ex.Message);
            return Results.Err<string>("Failed to process audio file.");
        }
    }

    public async Task<Result<string, string>> StripMp3MetadataAsync(string inputFilePath)
    {
        var outputFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        try
        {
            var metadataStripResult =
                await FFMpegArguments.FromFileInput(inputFilePath,
                                                    false)
                                     .OutputToFile(outputFilePath,
                                                   true,
                                                   options => options.CopyStreams()
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously();
            if (metadataStripResult.Success)
                return Results.Ok(outputFilePath);

            logger.LogError("Unable to strip metadata from MP3 file at {InputFilePath}", inputFilePath);
            return Results.Err<string>("Failed to process MP3 file.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                            "Unable to strip metadata from MP3 file at {InputFilePath}: {ErrorMessage}",
                            inputFilePath,
                            ex.Message);
            return Results.Err<string>("Failed to process MP3 file.");
        }
    }
}