﻿namespace LowPressureZone.Api.Constants;

internal static class PerformanceTypes
{
    public const string Live = "Live DJ Set";
    public const string Prerecorded = "Prerecorded DJ Set";

    public static IReadOnlyList<string> All => [Live, Prerecorded];
}
