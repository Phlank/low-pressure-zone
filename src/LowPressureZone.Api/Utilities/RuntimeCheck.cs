namespace LowPressureZone.Api.Utilities;

public static class RuntimeChecks
{
    public static bool IsTestRunner(this AppDomain domain) =>
        domain.GetAssemblies()
              .Select(assembly => assembly.FullName)
              .Any(name => name?.StartsWith("XUnit", StringComparison.InvariantCultureIgnoreCase) == true);
}