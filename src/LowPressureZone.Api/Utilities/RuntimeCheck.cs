namespace LowPressureZone.Api.Utilities;

public static class RuntimeChecks
{
    private static bool? _isTestRunner;
    
    public static bool IsTestRunner(this AppDomain domain)
    {
        if (_isTestRunner.HasValue) return _isTestRunner.Value;
        _isTestRunner = domain.GetAssemblies()
                              .Select(assembly => assembly.FullName)
                              .Any(name => name?.StartsWith("XUnit", StringComparison.InvariantCultureIgnoreCase) == true);
        return _isTestRunner.Value;
    }
}