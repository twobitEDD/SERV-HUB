namespace HTools
{
    // Class for synchronization update of input class with Unity updates 
    public class InputSyncBehaviour : IBehaviorSync
    {
        void IBehaviorSync.Start() { }
        void IBehaviorSync.Update() => InputHS.Update();
    }
}
