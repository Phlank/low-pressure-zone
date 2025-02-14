namespace LowPressureZone.Api.Constants;

public static class PerformanceTypes
{
    public const string Live = "Live DJ Set";
    public const string Prerecorded = "Prerecorded DJ Set";

    public static readonly IReadOnlyList<string> All = new List<string>()
    {
        Live, Prerecorded
    };
}
