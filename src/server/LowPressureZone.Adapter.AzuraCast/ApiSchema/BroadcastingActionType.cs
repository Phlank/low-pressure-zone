namespace LowPressureZone.Adapter.AzuraCast.ApiSchema;

public class BroadcastingActionType
{
    public string ActionPath { get; }

    private BroadcastingActionType(string actionPath)
    {
        ActionPath = actionPath;
    }
    
    public static readonly BroadcastingActionType Skip = new BroadcastingActionType("skip");
    public static readonly BroadcastingActionType Disconnect = new BroadcastingActionType("disconnect");
    public static readonly BroadcastingActionType Start = new BroadcastingActionType("start");
    public static readonly BroadcastingActionType Stop = new BroadcastingActionType("stop");
    public static readonly BroadcastingActionType Reload = new BroadcastingActionType("reload");
    public static readonly BroadcastingActionType Restart = new BroadcastingActionType("restart");
}