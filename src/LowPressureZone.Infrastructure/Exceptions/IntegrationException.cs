namespace LowPressureZone.Adapter.AzuraCast.Exceptions;

public class IntegrationException(string message) : InvalidOperationException(message);