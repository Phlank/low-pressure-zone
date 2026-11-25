namespace LowPressureZone.Adapter.AzuraCast.Exceptions;

public class AzuraCastStateException(string message) : InvalidOperationException(message);