using LowPressureZone.Api.Models.Configuration;
using LowPressureZone.Core;
using Microsoft.Extensions.Options;

namespace LowPressureZone.Api.Services;

public sealed partial class FormFileSaver(IOptions<FileConfiguration> fileConfig, ILogger<FormFileSaver> logger)
{
    private readonly string _temporaryLocation = fileConfig.Value.TemporaryLocation;

    public async Task<Result<string, string>> SaveFormFileAsync(IFormFile file, CancellationToken ct = default)
    {
        var path = Path.Combine(_temporaryLocation, Guid.NewGuid().ToString());

        try
        {
            Path.GetRandomFileName();
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            await using var fileStream = File.Create(path);
            await file.CopyToAsync(fileStream, ct);
        }
        catch (Exception ex)
        {
            LogSaveFailure(logger, ex.Message);
            return Result.Err<string, string>($"Failed to save uploaded file: {ex.Message}");
        }

        return Result.Ok(path);
    }

    [LoggerMessage(LogLevel.Error, $"{nameof(FormFileSaver)}: Failed to save uploaded file: {{errorMessage}}")]
    static partial void LogSaveFailure(ILogger<FormFileSaver> logger, string errorMessage);
}