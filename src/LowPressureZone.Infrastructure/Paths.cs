namespace LowPressureZone.Infrastructure;

public static class Paths
{
    private static readonly char[] InvalidFileNameChars = [.. Path.GetInvalidFileNameChars()];

    public static string CleanFileName(string fileName) =>
        new(fileName.Where(c => !InvalidFileNameChars.Contains(c)).ToArray());
}