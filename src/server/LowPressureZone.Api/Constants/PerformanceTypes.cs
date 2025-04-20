namespace LowPressureZone.Api.Constants;

public static class PerformanceTypes
{
    private const string Live = "Live DJ Set";
    private const string Prerecorded = "Prerecorded DJ Set";

    public static IReadOnlyList<string> All => [Live, Prerecorded];
}
