using LowPressureZone.Infrastructure;

namespace LowPressureZone.Api.Utilities;

public static class MixFile
{
    public static string GetMixFileName(string streamerName, DateTimeOffset startTime) =>
        $"{Paths.CleanFileName(streamerName)}_{startTime:yyyy-MM-dd_HH-mm-ss}.mp3";
}