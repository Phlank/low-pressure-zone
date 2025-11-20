using FFMpegCore;
using FFMpegCore.Enums;
using LowPressureZone.Core;

namespace LowPressureZone.Api.Services.Audio;

public sealed partial class Mp3Processor(ILogger<Mp3Processor> logger)
{
    public async Task<Result<bool, string>> ConvertFileToMp3Async(string inputFilePath, string outputFilePath)
    {
        try
        {
            var isConversionSuccessful =
                await FFMpegArguments.FromFileInput(inputFilePath,
                                                    false)
                                     .OutputToFile(outputFilePath,
                                                   overwrite: true,
                                                   options => options.WithAudioCodec(AudioCodec.LibMp3Lame)
                                                                     .WithAudioBitrate(320)
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously(throwOnError: true);

            if (isConversionSuccessful)
                return Result.Ok(true);

            LogUnableToConvertFile(logger, inputFilePath);
            return Result.Err<bool>("Failed to process audio file.");
        }
        catch (Exception ex)
        {
            LogUnableToConvertFileException(logger, inputFilePath, ex.Message);
            return Result.Err<bool>("Failed to process audio file.");
        }
    }

    public async Task<Result<bool, string>> StripMp3MetadataAsync(string inputFilePath, string outputFilePath)
    {
        try
        {
            var isMetadataStripSuccessful =
                await FFMpegArguments.FromFileInput(inputFilePath, 
                                                    false)
                                     .OutputToFile(outputFilePath, overwrite: true,
                                                   options => options.WithCopyCodec()
                                                                     .WithoutMetadata())
                                     .ProcessAsynchronously(throwOnError: true);
            if (isMetadataStripSuccessful)
                return Result.Ok(true);

            LogUnableToStripMetadata(logger, inputFilePath);
            return Result.Err<bool>("Failed to process MP3 file.");
        }
        catch (Exception ex)
        {
            LogUnableToStripMetadataException(logger, inputFilePath, ex.Message);
            return Result.Err<bool>("Failed to process MP3 file.");
        }
    }

    [LoggerMessage(LogLevel.Error, "Unable to convert file to MP3 at {inputFilePath}")]
    static partial void LogUnableToConvertFile(ILogger<Mp3Processor> logger, string inputFilePath);

    [LoggerMessage(LogLevel.Error, "Unable to convert file to MP3 at {inputFilePath}: {errorMessage}")]
    static partial void LogUnableToConvertFileException(
        ILogger<Mp3Processor> logger,
        string inputFilePath,
        string errorMessage);

    [LoggerMessage(LogLevel.Error, "Unable to strip metadata from MP3 file at {inputFilePath}")]
    static partial void LogUnableToStripMetadata(ILogger<Mp3Processor> logger, string inputFilePath);

    [LoggerMessage(LogLevel.Error, "Unable to strip metadata from MP3 file at {inputFilePath}: {errorMessage}")]
    static partial void LogUnableToStripMetadataException(
        ILogger<Mp3Processor> logger,
        string inputFilePath,
        string errorMessage);
}